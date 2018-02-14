using System.ComponentModel.DataAnnotations;

namespace StickManWebAPI.Models
{
	public class LoginModel
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public string DeviceId { get; set; }
	}
}