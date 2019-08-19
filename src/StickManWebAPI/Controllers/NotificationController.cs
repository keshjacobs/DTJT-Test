using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Models.Notification;
using StickManWebAPI.Models;
using StickManWebAPI.Models.Request;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Controllers
{
    public class NotificationController : ApiController
    {
        private readonly INotificationService _notificationService;
        private readonly IMessageService _messageService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ISessionService _sessionService;
        private readonly IFileService _fileService;

        public NotificationController(INotificationService notificationService,IMessageService messageService, ISessionService sessionService, IFileService fileService, IPushNotificationService pushNotificationService)
        {
            _notificationService = notificationService;
            _messageService = messageService;
            _sessionService = sessionService;
            _fileService = fileService;
            _pushNotificationService = pushNotificationService;
        }
        [HttpGet]
        public UnReadCount UnReadCount([FromUri]SessionData session) {

            _sessionService.Validate(session.UserId, session.SessionToken);
            int unreadNotification=_notificationService.GetUnreadNotificationCount(session.UserId);
            int unreadMessage = _messageService.GetUnreadMessagesCount(session.UserId);
            return new UnReadCount
            {
                replyCode = 200,
                replyMessage = $"Notification count.",
                FriendRequest=0,
                Notification=unreadNotification,
                Message= unreadMessage
            };
        }
        [HttpGet]
        public IEnumerable<NotificationTimeline> Timeline([FromUri]TimelineRequestModel timeline)
        {
            _sessionService.Validate(timeline.UserId, timeline.SessionToken);

            var messages = _notificationService.GetTimeline(timeline.UserId, timeline.Pagination.PageNumber, timeline.Pagination.PageSize);

            return messages;
        }

        [HttpPost]
        public Reply ReadMessage(int notificationId)
        {
            _notificationService.ReadNotification(notificationId);

            return new Reply
            {
                replyCode = 200,
                replyMessage = $"Notification {notificationId} was read"
            };
        }
    }
}
