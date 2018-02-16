using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickMan.Services.Models.Message;
using StickManWebAPI.Models;
using StickManWebAPI.Models.Request;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Controllers
{
	public class CastController : ApiController
	{
		private readonly ICastMessageService _castMessageService;
		private readonly ISessionService _sessionService;
		private readonly IFileService _fileService;
		private readonly IPushNotificationService _pushNotificationService;

		public CastController(ICastMessageService castMessageService, ISessionService sessionService, IFileService fileService, IPushNotificationService pushNotificationService)
		{
			_castMessageService = castMessageService;
			_sessionService = sessionService;
			_fileService = fileService;
			_pushNotificationService = pushNotificationService;
		}

		[HttpGet]
		public IEnumerable<CastMessage> Get([FromUri]CastPaginationModel pagination)
		{
			var messages = _castMessageService.GetMessages(pagination.PageNumber, pagination.PageSize, pagination.UserId);

			return messages;
		}

		[HttpGet]
		public IEnumerable<CastMessage> Search([FromUri]CastSearchModel model, string term)
		{
			if (model == null)
			{
				model = new CastSearchModel
				{
					Term = term
				};
			}

			var messages = _castMessageService.Search(model.Term, model.UserId);

			return messages;
		}

		[HttpPost]
		public Reply Send(CastMessageToUpload message)
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

			var castMessage = _castMessageService.Save(message.FileName, message.UserId, message.Title);
			_pushNotificationService.SendCastPush(message.UserId, castMessage);

			return new SendCastMessageReply(HttpStatusCode.OK, message.FileName)
			{
				CastMessageId = castMessage.MessageInfo.Id
			};
		}

		[HttpPost]
		public ClickCountReply Click(int castMessageId, int userId)
		{
			var clickCount = _castMessageService.ReadMessage(castMessageId, userId);

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
