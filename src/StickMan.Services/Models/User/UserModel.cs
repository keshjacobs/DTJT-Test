using System;

namespace StickMan.Services.Models.User
{
	public class UserModel
	{
		public int UserId { get; set; }
		public string UserName { get; set; }
		public string FullName { get; set; }
		public string MobileNo { get; set; }
		public string Email { get; set; }
		public DateTime? DOB { get; set; }
		public string Sex { get; set; }
		public string ImagePath { get; set; }
		public string DeviceId { get; set; }
	}
}
