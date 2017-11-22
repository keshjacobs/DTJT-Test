using System;
using System.Collections.Generic;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickMan.Services.Models.Message;
using StickManWebAPI.Models;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Controllers
{
	// TODO Remove obsolete endpoints
	public class MessageController : ApiController
	{
		private readonly IMessageService _messageService;
		private readonly ICastMessageService _castMessageService;
		private readonly ISessionService _sessionService;

		public MessageController(IMessageService messageService, ISessionService sessionService, ICastMessageService castMessageService)
		{
			_messageService = messageService;
			_sessionService = sessionService;
			_castMessageService = castMessageService;
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

		[Obsolete]
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

			_castMessageService.SaveMessage(audioContent.FilePath, audioContent.UserId, audioContent.Title);

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
