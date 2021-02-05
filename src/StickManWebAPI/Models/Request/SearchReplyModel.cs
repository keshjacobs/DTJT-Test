using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StickManWebAPI.Models.Request
{
    public class SearchReplyModel
    {
        public int OrginalPostId { get; set; }
        public int UserId { get; set; }
        public int RepliedUserId { get; set; }
    }
}