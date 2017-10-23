using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StickManWebAPI.Models
{
	public class userProfile
	{
		public string username { get; set; }
		public string fullName { get; set; }
		public string password { get; set; }
		public string mobileNo { get; set; }
		public string emailID { get; set; }
		public string dob { get; set; }
		public string sex { get; set; }
		public string imagePath { get; set; }
	}
}