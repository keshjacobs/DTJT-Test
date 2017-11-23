using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickMan.Services.Models.Message;
using StickManWebAPI.Models;
using StickManWebAPI.Models.Request;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Controllers
{
	// TODO Remove obsolete endpoints
	public class MessageController : ApiController
	{
		private readonly IMessageService _messageService;
		private readonly ICastMessageService _castMessageService;
		private readonly ISessionService _sessionService;
		private readonly IPathProvider _pathProvider;

		public MessageController(IMessageService messageService, ISessionService sessionService, ICastMessageService castMessageService, IPathProvider pathProvider)
		{
			_messageService = messageService;
			_sessionService = sessionService;
			_castMessageService = castMessageService;
			_pathProvider = pathProvider;
		}

	    [Obsolete]
		[HttpGet]
		public IEnumerable<TimelineModel> GetTimeline(int userId)
		{
			var timeline = _messageService.GetTimeline(userId, 0, 30);

			return timeline;
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
		public HttpResponseMessage Send(RegularMessageToUpload message)
		{
			if (string.IsNullOrEmpty(message.Base64Content))
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Message content is required");
			}

			try
			{
				_sessionService.Validate(message.UserId, message.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session");
			}

			var filePath = _pathProvider.BuildAudioPath(Path.Combine(message.UserId.ToString(), message.FileName));
			File.WriteAllBytes(filePath, Convert.FromBase64String(message.Base64Content));
			_messageService.Save(filePath, message.UserId, message.ReceiverId);

			return Request.CreateResponse(HttpStatusCode.OK, message.FileName);
		}

		[Obsolete]
		[HttpPost]
		public Reply SaveCastAudioPath(ObsoleteCastMessageToUpload audioContent)
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

			_castMessageService.Save(audioContent.FilePath, audioContent.UserId, audioContent.Title);

			return new Reply
			{
				replyCode = (int)EnumReply.processOk,
				replyMessage = "Cast message sucessfully saved"
			};
		}

		[Obsolete]
		[HttpPost]
		public ClickCountReply ClickOnCastMessage(int castMessageId)
		{
			var clickCount = _castMessageService.ReadMessage(castMessageId);

			return new ClickCountReply
			{
				replyCode = (int)EnumReply.processOk,
				replyMessage = "Cast message clicked",
				ClickCount = clickCount
			};
		}

		[Obsolete]
		[HttpGet]
		public IEnumerable<CastMessage> GetCastMessages([FromUri]PaginationModel pagination)
		{
			var messages = _castMessageService.GetMessages(pagination.PageNumber, pagination.PageSize);

			return messages;
		}
	}
}
