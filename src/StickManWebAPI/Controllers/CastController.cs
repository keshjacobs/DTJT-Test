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
	public class CastController : ApiController
	{
		private readonly ICastMessageService _castMessageService;
		private readonly ISessionService _sessionService;

		public CastController(ICastMessageService castMessageService, ISessionService sessionService)
		{
			_castMessageService = castMessageService;
			_sessionService = sessionService;
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

			_castMessageService.SaveMessage(audioContent.FilePath, audioContent.UserId, audioContent.Title);

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
				ClickCount = clickCount
			};
		}

		[HttpPost]
		public Reply ChangeTitle(ChangeTitleModel model)
		{
			try
			{
				_sessionService.Validate(model.UserId, model.SessionToken);
			}
			catch (InvalidSessionException ex)
			{
				return new Reply
				{
					replyCode = (int)EnumReply.processFail,
					replyMessage = ex.Message
				};
			}

			try
			{
				var title = _castMessageService.ChangeTitle(model.UserId, model.CastMessageId, model.NewTitle);

				return new ChangeTitleReply
				{
					Title = title
				};
			}
			catch (UnauthorizedAccessException)
			{
				return new Reply
				{
					replyCode = 400,
					replyMessage = "This cast message is not yours, you can not change its title"
				};
			}
		}
	}
}
