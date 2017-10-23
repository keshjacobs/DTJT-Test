using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StickManWebAPI.Models
{
	public class SearchUser
	{
		public string sessionToken{get; set;}
		public int userId{ get; set;}
		public string searchKeyword { get; set; }
	}
}