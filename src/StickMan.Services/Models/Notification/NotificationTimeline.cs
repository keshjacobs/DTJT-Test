using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickMan.Services.Models.Notification
{
    public class NotificationTimeline
    {
        public int NotificationId { get; set; }
        public int PostId { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public string TimePassed { get; set; }
        public bool ReadStatus { get; set; }
    }
}
