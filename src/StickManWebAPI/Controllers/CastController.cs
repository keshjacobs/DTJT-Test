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
	public class CastController : ApiController
	{
		private readonly ICastMessageService _castMessageService;
		private readonly ISessionService _sessionService;
		private readonly IFileService _fileService;

		public CastController(ICastMessageService castMessageService, ISessionService sessionService, IFileService fileService)
		{
			_castMessageService = castMessageService;
			_sessionService = sessionService;
			_fileService = fileService;
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

			var id = _castMessageService.Save(message.FileName, message.UserId, message.Title);

			return new SendCastMessageReply(HttpStatusCode.OK, message.FileName)
			{
				CastMessageId = id
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
