using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StickManWebAPI.Models
{
    public class ResetPasswordModel
    {
        public int UserId { get; set; }

        public string SessionToken { get; set; }
    }
}