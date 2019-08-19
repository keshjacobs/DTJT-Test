using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Extensions;
using StickMan.Services.Models;
using StickMan.Services.Models.Notification;
using StickMan.Services.Models.Push;

namespace StickMan.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICastMessageService _castMessageService;
        public NotificationService(IUnitOfWork unitOfWork, IUserService userService, ICastMessageService castMessageService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _castMessageService = castMessageService;
        }
        public void CleanUpNotification()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NotificationTimeline> GetTimeline(int userId, int page, int size)
        {
            var timeline = new List<NotificationTimeline>();

            var notificationInfo = _unitOfWork.Repository<Notification>()
                .GetQuery(x => x.RecieverId == userId)
                .OrderByDescending(m => m.NotifacationTime)
                .Skip(page * size)
                .Take(size)
                .ToList();

            foreach (var notification in notificationInfo)
            {
                var timelineMessage = MapToTimeLine(notification);

                timeline.Add(timelineMessage);
            }

            return timeline;
        }

        public int GetUnreadNotificationCount(int userId)
        {
            return _unitOfWork.Repository<Notification>()
                .Count(m => m.RecieverId == userId && !m.ReadStatus && !m.DeleteStatus);
        }

        public void ReadNotification(int id)
        {
            var notification = _unitOfWork.Repository<Notification>().GetSingle(x => x.NotificationId == id);

            if (!notification.ReadStatus)
            {
                notification.ReadStatus = true;
                _unitOfWork.Repository<Notification>().Update(notification);
                _unitOfWork.Save();
            }
        }

        public void SaveCastNotification(int userId, int postId, NotificationType type)
        {
            string message;
            var castMessage = _castMessageService.GetMessage(postId);
            var receiver = _userService.GetUser(castMessage.UserID.Value);
            var sender = _userService.GetUser(userId);
            switch (type)
            {
                case NotificationType.Repost:
                    message = $"{sender.FullName} RESPOSTED your Cast";
                    break;
                case NotificationType.Replay:
                    message = $"{sender.FullName} REPLIED your Cast";
                    break;
                case NotificationType.Like:
                    message = $"{sender.FullName} LIKED your Cast";
                    break;
                case NotificationType.DisLike:
                    message = $"{sender.FullName} DISLIKED your Cast";
                    break;
                default:
                    message = $"Notification From {sender.FullName}";
                    break;
            }
            SendNotification(postId, type, userId, receiver.UserId, message, castMessage.Title);
        }

        public void SaveFriendRequestNotification(int senderId, FriendRequestDto friendRequest)
        {
            throw new NotImplementedException();
        }

        private void SendNotification(int PostId, NotificationType type, int senderId, int recieverId, string message, string title)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(title) || recieverId == 0)
            {
                return;
            }

            var notification = new Notification
            {
                PostId = PostId,
                Type = (int)type,
                SenderId = senderId,
                RecieverId = recieverId,
                Message = message,
                Title = title,
                ReadStatus = false,
                DeleteStatus = false,
                NotifacationTime = DateTime.Now

            };

            _unitOfWork.Repository<Notification>().Insert(notification);
            _unitOfWork.Save();
        }
        private NotificationTimeline MapToTimeLine(Notification notification) {
            var timeline = new NotificationTimeline
            {
                NotificationId= notification.NotificationId,
                PostId = notification.PostId,
                Message = notification.Message,
                Title = notification.Title,
                ReadStatus = notification.ReadStatus,
                TimePassed = (DateTime.UtcNow - notification.NotifacationTime).FormatDuration()
            };
            return timeline;
         }
    }
}
