using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Hangfire;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickMan.Services.Models.Message;
using StickMan.Services.Models.Push;
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
        private readonly INotificationService _notificationService;

        public CastController(ICastMessageService castMessageService, ISessionService sessionService, IFileService fileService, IPushNotificationService pushNotificationService, INotificationService notificationService)
		{
			_castMessageService = castMessageService;
			_sessionService = sessionService;
			_fileService = fileService;
			_pushNotificationService = pushNotificationService;
            _notificationService = notificationService;
		}

		[HttpGet]
		public IEnumerable<CastMessage> Get([FromUri]CastPaginationModel pagination)
		{
            if (pagination.PostId.HasValue) {
               return _castMessageService.GetMessages(pagination.PageNumber, pagination.PageSize, pagination.UserId,pagination.PostId.Value);
            }
            return _castMessageService.GetMessages(pagination.PageNumber, pagination.PageSize, pagination.UserId);
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

        [HttpPost]
        public LikePostReply LikePost(int castMessageId, int userId)
        {
            var likeCount = _castMessageService.LikeMessage(castMessageId, userId);
            _notificationService.SaveCastNotification(userId, castMessageId, NotificationType.Like);
            return new LikePostReply
            {
                LikePostCount = likeCount
            };
        }


        [HttpPost]
        public DislikePostReply DislikePost(int castMessageId, int userId)
        {
            var likeCount = _castMessageService.DislikeMessage(castMessageId, userId);
            _notificationService.SaveCastNotification(userId, castMessageId, NotificationType.DisLike);
            return new DislikePostReply
            {
                DislikePostCount = likeCount
            };
        }

        [HttpPost]
        public Reply Repost(RepostModel model)
        {
            try
            {
                _sessionService.Validate(model.UserId, model.SessionToken);
            }
            catch (InvalidSessionException)
            {
                return new Reply(HttpStatusCode.BadRequest, "Invalid session");
            }

            var castMessage = _castMessageService.Repost(model.UserId, model.CastMessageId, model.Comment);
            _notificationService.SaveCastNotification(model.UserId, model.CastMessageId, NotificationType.Repost);
            return new SendCastMessageReply(HttpStatusCode.OK, "Repost message clicked")
            {
                CastMessageId = castMessage.MessageInfo.Id
            };
        }

        [HttpPost]
        public Reply ReplyPost(ReplyPostModel model)
        {
            if (string.IsNullOrEmpty(model.Base64Content))
            {
                return new Reply(HttpStatusCode.BadRequest, "Message content is required");
            }

            try
            {
                _sessionService.Validate(model.UserId, model.SessionToken);
            }
            catch (InvalidSessionException)
            {
                return new Reply(HttpStatusCode.BadRequest, "Invalid session");
            }

            _fileService.SaveFile(model.UserId, model.FileName, model.Base64Content);

            var message = _castMessageService.GetMessage(model.CastMessageId);

            var castMessage = _castMessageService.Save(model.FileName, model.UserId, message.Title, model.CastMessageId);
            var repost = _castMessageService.Repost(model.UserId, castMessage.MessageInfo.Id, model.Comment, model.CastMessageId);
            _notificationService.SaveCastNotification(model.UserId, model.CastMessageId, NotificationType.Replay);
            return new SendCastMessageReply(HttpStatusCode.OK, model.FileName)
            {
                CastMessageId = castMessage.MessageInfo.Id
            };
        }
    }
}
