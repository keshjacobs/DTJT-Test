using System.Collections.Generic;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickMan.Services.Models.Message;
using StickManWebAPI.Models;

namespace StickManWebAPI.Controllers
{
	public class CastController : ApiController
	{
		private readonly ICastMessageService _castMessageService;
		private readonly ISessionService _sessionService;

		public CastController(ICastMessageService castMessageService, ISessionService sessionService)
		{
			_castMessageService = castMessageService;
			_sessionService = sessionService;
		}

		[HttpPost]
		public Reply SaveAudioPath(CastAudioContent audioContent)
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

			_castMessageService.SaveMessage(audioContent.FilePath, audioContent.UserId);

			return new Reply
			{
				replyCode = (int)EnumReply.processOk,
				replyMessage = "Cast message sucessfully saved"
			};
		}

		[HttpPost]
		public ClickCountReply Click(int castMessageId)
		{
			var clickCount = _castMessageService.ReadMessage(castMessageId);

			return new ClickCountReply
			{
				replyCode = (int)EnumReply.processOk,
				replyMessage = "Cast message clicked",
				ClickCount = clickCount
			};
		}

		[HttpGet]
		public IEnumerable<CastMessage> Get([FromUri]PaginationModel pagination)
		{
			var messages = _castMessageService.GetMessages(pagination.PageNumber, pagination.PageSize);

			return messages;
		}

		[HttpGet]
		public IEnumerable<CastMessage> Search(string term)
		{
			var messages = _castMessageService.Search(term);

			return messages;
		}
	}
}
