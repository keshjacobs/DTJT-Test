using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StickMan.Services.Models;
using StickMan.Services.Models.Notification;
using StickMan.Services.Models.Push;

namespace StickMan.Services.Contracts
{
    public interface INotificationService
    {
        int GetUnreadNotificationCount(int userId);

        //ICollection<JustSentMessage> Save(string filePath, int userId, IEnumerable<int> receiverIds);

        IEnumerable<NotificationTimeline> GetTimeline(int userId, int page, int size);
        void SaveFriendRequestNotification(int senderId, FriendRequestDto friendRequest);
        void SaveCastNotification(int userId, int postId, NotificationType type);
        void ReadNotification(int id);

        void CleanUpNotification();
    }
}
