using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StickManWebAPI.Models
{
	public class SignUpModel
	{
		public string Username { get; set; }

		public string FullName { get; set; }

		public string Password { get; set; }

		public string MobileNo { get; set; }

		public string EmailID { get; set; }

		public string Dob { get; set; }

		public string Sex { get; set; }

		public string ImagePath { get; set; }

		public string DeviceId { get; set; }
	}
}