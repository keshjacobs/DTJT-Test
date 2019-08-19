using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StickManWebAPI.Models.Response
{
    public class UnReadCount: Reply
    {
        public int Notification { get; set; }
        public int Message { get; set; }
        public int FriendRequest { get; set; }
    }
}