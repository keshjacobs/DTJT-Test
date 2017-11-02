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
			var timeline = _messageService.GetTimeline(userId);

			return timeline;
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
	}
}
