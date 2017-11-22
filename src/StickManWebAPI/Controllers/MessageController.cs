using System.Collections.Generic;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickMan.Services.Models.Message;
using StickManWebAPI.Models;

namespace StickManWebAPI.Controllers
{
	public class MessageController : ApiController
	{
		private readonly IMessageService _messageService;
		private readonly ISessionService _sessionService;

		public MessageController(IMessageService messageService, ISessionService sessionService)
		{
			_messageService = messageService;
			_sessionService = sessionService;
		}

		[HttpGet]
		public IEnumerable<TimelineModel> GetTimeline(int userId)
		{
			var timeline = _messageService.GetTimeline(userId, 0, 30);

			return timeline;
		}

		[HttpGet]
		public IEnumerable<TimelineModel> GetPagedTimeline([FromUri]TimelineRequestModel timeline)
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
		public Reply SaveCastAudioPath(CastAudioContent audioContent)
		{
			try
			{
				_sessionService.Validate(audioContent.UserId, audioContent.SessionToken);
			}
			catch (InvalidSessionException ex)
			{
				return new Reply
				{
					replyCode = (int)EnumReply.processFail,
					replyMessage = ex.Message
				};
			}

			_messageService.SaveCastMessage(audioContent.FilePath, audioContent.UserId);

			return new Reply
			{
				replyCode = (int)EnumReply.processOk,
				replyMessage = "Cast message sucessfully saved"
			};
		}

		[HttpPost]
		public ClickCountReply ClickOnCastMessage(int castMessageId)
		{
			var clickCount = _messageService.IncreaseCastClickCount(castMessageId);

			return new ClickCountReply
			{
				replyCode = (int)EnumReply.processOk,
				replyMessage = "Cast message clicked",
				ClickCount = clickCount
			};
		}

		[HttpGet]
		public IEnumerable<CastMessage> GetCastMessages([FromUri]PaginationModel pagination)
		{
			var messages = _messageService.GetCastMessages(pagination.PageNumber, pagination.PageSize);

			return messages;
		}
	}
}
