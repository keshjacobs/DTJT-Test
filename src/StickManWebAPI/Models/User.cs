using System;
using System.Collections.Generic;
using StickManWebAPI.Models.Response;

namespace StickManWebAPI.Models
{
	public class UserWrapper
	{
		public Reply reply { get; set; }
		public User user { get; set; }
	}

	public class SearchResult
	{
		public Reply reply { get; set; }
		public List<UserExtension> users { get; set; }
	}

	public class FriendRequest
	{
		public int FriendRequestId { get; set; }
		public string FriendRequestState { get; set; }
	}

	public class SendFriendRequest
	{
		public Reply reply { get; set; }
		public FriendRequest FriendRequestDetail { get; set; }
		public User user { get; set; }
	}

	public class User
	{
		public Int32 userID { get; set; }
		public string sessionToken { get; set; }
		public string username { get; set; }
		public string fullName { get; set; }
		public string mobileNo { get; set; }
		public string sex { get; set; }
		public string emailID { get; set; }
		public string dob { get; set; }
		public string imagePath { get; set; }
		public string deviceId { get; set; }
	}

	public class UserExtension : User
	{
		public int FriendRequestID { get; set; }
		public string FriendRequestStatus { get; set; }
	}
}