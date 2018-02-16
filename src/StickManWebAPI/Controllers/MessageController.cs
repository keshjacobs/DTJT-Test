using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickMan.Services.Models.Message;
using StickManWebAPI.Models;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Controllers
{
	public class MessageController : ApiController
	{
		private readonly IMessageService _messageService;
		private readonly IPushNotificationService _pushNotificationService;
		private readonly ISessionService _sessionService;
		private readonly IFileService _fileService;

		public MessageController(IMessageService messageService, ISessionService sessionService, IFileService fileService, IPushNotificationService pushNotificationService)
		{
			_messageService = messageService;
			_sessionService = sessionService;
			_fileService = fileService;
			_pushNotificationService = pushNotificationService;
		}

		[HttpGet]
		public IEnumerable<TimelineModel> Timeline([FromUri]TimelineRequestModel timeline)
		{
			_sessionService.Validate(timeline.UserId, timeline.SessionToken);

			var messages = _messageService.GetTimeline(timeline.UserId, timeline.Pagination.PageNumber, timeline.Pagination.PageSize);

			return messages;
		}

		[HttpPost]
		public Reply ReadMessage(int messageId)
		{
			_messageService.ReadMessage(messageId);

			return new Reply
			{
				replyCode = 200,
				replyMessage = $"Message {messageId} was read"
			};
		}

		[HttpPost]
		public Reply Send(RegularMessageToUpload message)
		{
			if (string.IsNullOrEmpty(message.Base64Content))
			{
				return new Reply(HttpStatusCode.BadRequest, "Message content is required");
			}

			try
			{
				_sessionService.Validate(message.UserId, message.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return new Reply(HttpStatusCode.BadRequest, "Invalid session");
			}

			_fileService.SaveFile(message.UserId, message.FileName, message.Base64Content);

			var messages = _messageService.Save(message.FileName, message.UserId, message.ReceiverIds);
			_pushNotificationService.SendMessagePush(message.UserId, message.ReceiverIds, messages);

			return new SendMessageReply(HttpStatusCode.OK, message.FileName)
			{
				MessageIds = messages.Select(x => x.MessageId)
			};
		}
	}
}
