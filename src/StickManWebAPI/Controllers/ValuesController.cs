using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Http;
using StickManWebAPI.Models;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Security.Authentication;
using PushSharp.Apple;
using PushSharp;
using PushSharp.Core;
using Newtonsoft.Json.Linq;

namespace StickManWebAPI.Controllers
{
	public class ValuesController : ApiController
	{
		// GET api/values
		//public IEnumerable<string> Get()
		public string Get()
		{
			//string str = PushSharpNotification();

			return "ff";
			//return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
		}

		[HttpPost]
		public UserWrapper signUp(string username, string fullName, string password, string mobileNo, string emailID, string dob, string sex, string imagePath, string deviceId)
		{
			UserWrapper userWrapper = new UserWrapper();
			Reply reply = new Reply();
			User user = new User();

			try
			{
				//required field checks
				if (!string.IsNullOrWhiteSpace(username))
				{
					if (!string.IsNullOrWhiteSpace(emailID))
					{
						if (!string.IsNullOrWhiteSpace(password))
						{
							SqlConnection con = new SqlConnection();
							con.ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString;

							SqlCommand cmd = new SqlCommand();
							cmd.CommandType = System.Data.CommandType.StoredProcedure;
							cmd.Connection = con;

							cmd.CommandText = "StickMan_usp_CreateUpdate_User";
							cmd.Parameters.Add("@UserID", System.Data.SqlDbType.Int).Value = 0;
							cmd.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar, 500).Value = username;
							cmd.Parameters.Add("@FullName", System.Data.SqlDbType.VarChar, 500).Value = fullName;
							cmd.Parameters.Add("@Password", System.Data.SqlDbType.VarChar, 500).Value = password;
							cmd.Parameters.Add("@MobileNo", System.Data.SqlDbType.VarChar, 500).Value = mobileNo;
							cmd.Parameters.Add("@EmailID", System.Data.SqlDbType.VarChar, 500).Value = emailID;
							cmd.Parameters.Add("@DOB", System.Data.SqlDbType.VarChar,100).Value = dob;
							cmd.Parameters.Add("@Sex", System.Data.SqlDbType.VarChar, 500).Value = sex;
							cmd.Parameters.Add("@ImagePath", System.Data.SqlDbType.VarChar, 1024).Value = imagePath;
							cmd.Parameters.Add("@DeviceId", System.Data.SqlDbType.VarChar, 1024).Value = deviceId;

							SqlDataAdapter adp = new SqlDataAdapter(cmd);
							DataSet ds = new DataSet();
							adp.Fill(ds);

							user.userID = Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]);
							user.sessionToken = ds.Tables[0].Rows[0]["Token"].ToString();
							reply.replyCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]);
							reply.replyMessage = ds.Tables[0].Rows[0]["Message"].ToString();

							if (Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]) == 200)
							{
								user.username = ds.Tables[0].Rows[0]["username"].ToString();
								user.fullName = ds.Tables[0].Rows[0]["FullName"].ToString();
								user.mobileNo = ds.Tables[0].Rows[0]["MobileNo"].ToString();
								user.emailID = ds.Tables[0].Rows[0]["EmailID"].ToString();
								user.dob = ds.Tables[0].Rows[0]["DOB"].ToString();
								user.sex = ds.Tables[0].Rows[0]["Sex"].ToString();
								user.imagePath = ds.Tables[0].Rows[0]["imagePath"].ToString();
								user.deviceId = ds.Tables[0].Rows[0]["DeviceId"].ToString();
							}

							userWrapper.user = user;

							if (reply.replyCode == 200)
							{
								string mainDIR = @"~\Content\Audio\" + user.userID;

								if (!Directory.Exists(mainDIR))
								{
									Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath(mainDIR));
									//System.Web.Hosting.HostingEnvironment.MapPath(mainDIR);
								}
							}

						}
						else
						{
							reply.replyCode = 305;
							reply.replyMessage = "Password required";
						}
					}
					else
					{
						reply.replyCode = 304;
						reply.replyMessage = "Email ID required";
					}
				}
				else
				{
					reply.replyCode = 303;
					reply.replyMessage = "Username required";
				}
			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message;
			}

			userWrapper.reply = reply;
			return userWrapper;
		}

		[HttpPost]
		public UserWrapper Login(string username, string password, string deviceId)
		{
			UserWrapper userWrapper = new UserWrapper();
			Reply reply = new Reply();
			User user = new User();
			try
			{
				SqlConnection con = new SqlConnection();
				con.ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString;

				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Connection = con;
				cmd.CommandText = "StickMan_usp_Login_User";
				cmd.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar, 32).Value = username;
				cmd.Parameters.Add("@Password", System.Data.SqlDbType.VarChar, 1024).Value = password;
				cmd.Parameters.Add("@DeviceId", System.Data.SqlDbType.VarChar, 1024).Value = deviceId;
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				adp.Fill(ds);

				user.userID = Convert.ToInt32(ds.Tables[0].Rows[0]["UserId"]);
				user.sessionToken = ds.Tables[0].Rows[0]["Token"].ToString();

				reply.replyCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]);
				reply.replyMessage = ds.Tables[0].Rows[0]["Message"].ToString();

				if (Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]) == 200)
				{
					user.username = ds.Tables[0].Rows[0]["username"].ToString();
					user.fullName = ds.Tables[0].Rows[0]["FullName"].ToString();
					user.mobileNo = ds.Tables[0].Rows[0]["MobileNo"].ToString();
					user.emailID = ds.Tables[0].Rows[0]["EmailID"].ToString();
					user.dob = ds.Tables[0].Rows[0]["DOB"].ToString();
					user.sex = ds.Tables[0].Rows[0]["Sex"].ToString();
					user.imagePath = ds.Tables[0].Rows[0]["imagePath"].ToString();
					user.deviceId = ds.Tables[0].Rows[0]["DeviceId"].ToString();

				}

				userWrapper.user = user;
			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message;
			}

			userWrapper.reply = reply;
			return userWrapper;
		}

		[HttpPost]
		public AudioWrapper SaveAudioPath(AudioContent audioContent)
		{
			AudioWrapper audioWrapper = new AudioWrapper();
			//List<AudioWrapper> audioWrapperList = new List<AudioWrapper>();
			Reply reply = new Reply();
			PushInfo pushInfo = new PushInfo();


			try
			{
				SqlConnection con = new SqlConnection();
				con.ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString;
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Connection = con;
				cmd.CommandText = "StickMan_usp_SaveAudioPath";
				foreach (string currentrecieverId in audioContent.recieverId)
				{

					cmd.Parameters.Add("@UserID", System.Data.SqlDbType.Int, 32).Value = audioContent.userId;
					cmd.Parameters.Add("@RecieverID", System.Data.SqlDbType.Int, 32).Value = currentrecieverId;
					cmd.Parameters.Add("@FilePath", System.Data.SqlDbType.VarChar, 2048).Value = audioContent.filePath;
					cmd.Parameters.Add("@Filter", System.Data.SqlDbType.VarChar, 2048).Value = audioContent.filter;
					cmd.Parameters.Add("@SessionToken", System.Data.SqlDbType.VarChar, 512).Value = audioContent.sessionToken;
					SqlDataAdapter adp = new SqlDataAdapter(cmd);
					DataSet ds = new DataSet();
					adp.Fill(ds);
					cmd.Parameters.Clear();
					reply.replyCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]);
					reply.replyMessage = ds.Tables[0].Rows[0]["ResponseMesssage"].ToString();
					pushInfo.pushStatus = string.Empty;
					//push message to reciever
					if (reply.replyCode == 200)
					{
						string deviceID = ds.Tables[0].Rows[0]["DeviceID"].ToString();

						if (!string.IsNullOrEmpty(deviceID))
						{
							PushSharpNotification(deviceID, "New audio file recieved!!");
							pushInfo.pushStatus = "Sent";
						}
						else
						{
							pushInfo.pushStatus = "Not sent. Device Info required.";
						}
					}


					//audioWrapper.pushInfo = pushInfo;


					//audioWrapperList.Add(audioWrapper);
				}
			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message;
			}

			audioWrapper.reply = reply;
			return audioWrapper;
		}


        //[HttpGet]
        //public SendFriendRequest SendFriendRequest(int UserId, int RecieverUserId, string SessionToken)
       [HttpPost]
        public SendFriendRequest SendFriendRequest(Friend friend)
             
		{
			SendFriendRequest response = new SendFriendRequest();
			Reply reply = new Reply();
			FriendRequest requestDetails = new FriendRequest();
			User users = new User();
			string deviceID = string.Empty;
			User user = new User();

			try
			{
				SqlConnection con = new SqlConnection();
				con.ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString;

				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Connection = con;
				cmd.CommandText = "StickMan_usp_SendFriendRequest";
                cmd.Parameters.Add("@SenderID", System.Data.SqlDbType.Int, 32).Value = friend.UserId;
                cmd.Parameters.Add("@RecieverID", System.Data.SqlDbType.Int, 32).Value = friend.RecieverUserId;
                cmd.Parameters.Add("@SessionToken", System.Data.SqlDbType.VarChar).Value = friend.SessionToken;
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				adp.Fill(ds);

				reply.replyCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]);
				reply.replyMessage = ds.Tables[0].Rows[0]["ResponseMesssage"].ToString();
				deviceID = ds.Tables[0].Rows[0]["DeviceID"].ToString();

				requestDetails.FriendRequestId = Convert.ToInt32(ds.Tables[0].Rows[0]["FriendRequestId"]);
				requestDetails.FriendRequestState = ds.Tables[0].Rows[0]["FriendRequestState"].ToString();

				response.FriendRequestDetail = requestDetails;



				//creating user object
				if (reply.replyCode == 200 && ds.Tables[1].Rows.Count > 0)
				{


					foreach (DataRow record in ds.Tables[1].Rows)
					{

						user.userID = Convert.ToInt32(record["UserID"]);
						user.username = string.IsNullOrEmpty(record["UserName"].ToString()) ? string.Empty : record["UserName"].ToString();
						user.fullName = string.IsNullOrEmpty(record["FullName"].ToString()) ? string.Empty : record["FullName"].ToString();
						user.sex = string.IsNullOrEmpty(record["Sex"].ToString()) ? string.Empty : record["Sex"].ToString();
						user.imagePath = string.IsNullOrEmpty(record["ImagePath"].ToString()) ? string.Empty : record["ImagePath"].ToString();
						user.sessionToken = string.Empty;
						user.mobileNo = string.IsNullOrEmpty(record["MobileNo"].ToString()) ? string.Empty : record["MobileNo"].ToString();
						user.emailID = string.IsNullOrEmpty(record["EmailID"].ToString()) ? string.Empty : record["EmailID"].ToString();
						user.dob = string.IsNullOrEmpty(record["DOB"].ToString()) ? string.Empty : record["DOB"].ToString();
						user.deviceId = string.Empty;

						//usersList.Add(user);
					}

					//send push notification
					if (Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]) == 200)
					{
						string message = user.username + " sent you a friend request.";

						if (!string.IsNullOrEmpty(deviceID))
							PushSharpNotification(deviceID, message);
					}

					response.user = user;
				}
				else if (reply.replyCode == 200 && ds.Tables[1].Rows.Count == 0)
				{
					reply.replyCode = Convert.ToInt32(EnumReply.noDataFound);
					reply.replyMessage = "No Data Found";
				}



			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message;
			}

			response.reply = reply;
			return response;
		}

		[HttpPost]
		public SearchResult SearchUsers(SearchUser searchUser)
		{
			SearchResult searchResult = new SearchResult();
			Reply reply = new Reply();
			List<UserExtension> usersList = new List<UserExtension>();


			try
			{
				SqlConnection con = new SqlConnection();
				con.ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString;

				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Connection = con;
				cmd.CommandText = "StickMan_usp_GetUsersList";
				cmd.Parameters.Add("@SessionToken", System.Data.SqlDbType.VarChar).Value = searchUser.sessionToken;
				cmd.Parameters.Add("@UserId", System.Data.SqlDbType.Int, 32).Value = searchUser.userId;
				cmd.Parameters.Add("@SearchKeyword", System.Data.SqlDbType.VarChar).Value = searchUser.searchKeyword;
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				adp.Fill(ds);

				reply.replyCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]);
				reply.replyMessage = ds.Tables[0].Rows[0]["ResponseMesssage"].ToString();

				//          U.UserID, U.UserName, U.FullName, U.ImagePath,U.Sex, FriendRequestID = ISNULL(FR.FriendRequestID,0),   
				//FriendRequestStatus = CASE WHEN FR.FriendRequestStatus = 0 THEN 'Pending'   
				//      ELSE CASE WHEN FR.FriendRequestStatus = 1 THEN 'Accepted'  
				//      ELSE 'Not Sent' END END  
				//      FROM royal_mutiyar.[StickMan_Users] U  

				if (reply.replyCode == 200 && ds.Tables[1].Rows.Count > 0)
				{
					foreach (DataRow record in ds.Tables[1].Rows)
					{
						UserExtension user = new UserExtension();
						user.userID = Convert.ToInt32(record["UserID"]);
						user.username = string.IsNullOrEmpty(record["UserName"].ToString()) ? string.Empty : record["UserName"].ToString();
						user.fullName = string.IsNullOrEmpty(record["FullName"].ToString()) ? string.Empty : record["FullName"].ToString(); ;
						user.sex = string.IsNullOrEmpty(record["Sex"].ToString()) ? string.Empty : record["Sex"].ToString(); ; ;
						user.imagePath = string.IsNullOrEmpty(record["ImagePath"].ToString()) ? string.Empty : record["ImagePath"].ToString(); ; ; ;
						//user.FriendRequestID = Convert.ToInt32(record["FriendRequestID"]);
						user.FriendRequestStatus = string.Empty;
						//user.FriendRequestStatus = string.IsNullOrEmpty(record["FriendRequestStatus"].ToString()) ? string.Empty : record["FriendRequestStatus"].ToString();
						user.sessionToken = string.Empty;
						user.mobileNo = string.IsNullOrEmpty(record["MobileNo"].ToString()) ? string.Empty : record["MobileNo"].ToString();
						user.emailID = string.IsNullOrEmpty(record["EmailID"].ToString()) ? string.Empty : record["EmailID"].ToString();
						user.dob = string.IsNullOrEmpty(record["DOB"].ToString()) ? string.Empty : record["DOB"].ToString();
						user.deviceId = string.Empty;

						usersList.Add(user);
					}
				}
				else if (reply.replyCode == 200 && ds.Tables[1].Rows.Count == 0)
				{
					reply.replyCode = Convert.ToInt32(EnumReply.noDataFound);
					reply.replyMessage = "No Data Found";
				}

				searchResult.users = usersList;
			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message;
			}

			searchResult.reply = reply;
			return searchResult;
		}

		[HttpPost]
		public Reply RespondFriendRequest(Friend friend)
		{
			Reply reply = new Reply();

			try
			{
				SqlConnection con = new SqlConnection();
				con.ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString;

				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Connection = con;
				cmd.CommandText = "StickMan_usp_ResponseFriendRequest";
				cmd.Parameters.Add("@UserID", System.Data.SqlDbType.Int, 32).Value = friend.UserId;
				cmd.Parameters.Add("@RespondingToUserID", System.Data.SqlDbType.Int, 32).Value = friend.RespondingToUserID;
				cmd.Parameters.Add("@FriendRequestID", System.Data.SqlDbType.Int, 32).Value = friend.FriendRequestId;
				cmd.Parameters.Add("@FriendRequestReply", System.Data.SqlDbType.Int, 32).Value = friend.FriendRequestReply;
				cmd.Parameters.Add("@SessionToken", System.Data.SqlDbType.VarChar).Value = friend.SessionToken;
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				adp.Fill(ds);

				reply.replyCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]);
				reply.replyMessage = ds.Tables[0].Rows[0]["ResponseMesssage"].ToString();

			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message;
			}

			return reply;
		}

		[HttpPost]
		public SearchResult GetFriends(Friend friend)
		{
			SearchResult searchResult = new SearchResult();
			Reply reply = new Reply();
			List<UserExtension> usersList = new List<UserExtension>();

			try
			{
				SqlConnection con = new SqlConnection();
				con.ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString;

				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Connection = con;
				cmd.CommandText = "StickMan_usp_GetFriends";
				cmd.Parameters.Add("@UserID", System.Data.SqlDbType.Int, 32).Value = friend.UserId;
				cmd.Parameters.Add("@SessionToken", System.Data.SqlDbType.VarChar).Value = friend.SessionToken;
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				adp.Fill(ds);

				reply.replyCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]);
				reply.replyMessage = ds.Tables[0].Rows[0]["ResponseMesssage"].ToString();

				if (reply.replyCode == 200 && ds.Tables[1].Rows.Count > 0)
				{
					foreach (DataRow record in ds.Tables[1].Rows)
					{
						UserExtension user = new UserExtension();
						user.userID = Convert.ToInt32(record["UserID"]);
						user.username = record["UserName"].ToString();
						user.fullName = record["FullName"].ToString();
						user.imagePath = record["ImagePath"].ToString();
						user.sex = record["Sex"].ToString();
						user.mobileNo = record["MobileNo"].ToString();
						user.emailID = record["EmailID"].ToString();
						user.dob = record["DOB"].ToString();
						usersList.Add(user);
					}

					searchResult.users = usersList;
				}
				else if (reply.replyCode == 200 && ds.Tables[1].Rows.Count == 0)
				{
					reply.replyCode = Convert.ToInt32(EnumReply.noDataFound);
					reply.replyMessage = "No Data Found";
				}

			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message;
			}

			searchResult.reply = reply;
			return searchResult;
		}

		[HttpPost]
		public SearchResult GetPendingFriendRequests(Friend friend)
		{
			SearchResult searchResult = new SearchResult();
			Reply reply = new Reply();
			List<UserExtension> usersList = new List<UserExtension>();

			try
			{
				SqlConnection con = new SqlConnection();
				con.ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString;

				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Connection = con;
				cmd.CommandText = "StickMan_usp_GetPendingFriendRequests";
				cmd.Parameters.Add("@SessionToken", System.Data.SqlDbType.VarChar).Value = friend.SessionToken;
				cmd.Parameters.Add("@UserId", System.Data.SqlDbType.Int, 32).Value = friend.UserId;

				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				adp.Fill(ds);

				reply.replyCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]);
				reply.replyMessage = ds.Tables[0].Rows[0]["ResponseMesssage"].ToString();

				if (reply.replyCode == 200 && ds.Tables[1].Rows.Count > 0)
				{
					foreach (DataRow record in ds.Tables[1].Rows)
					{
						UserExtension user = new UserExtension();
						user.userID = Convert.ToInt32(record["UserID"]);
						user.username = string.IsNullOrEmpty(record["UserName"].ToString()) ? string.Empty : record["UserName"].ToString();
						user.fullName = string.IsNullOrEmpty(record["FullName"].ToString()) ? string.Empty : record["FullName"].ToString(); ;
						user.sex = string.IsNullOrEmpty(record["Sex"].ToString()) ? string.Empty : record["Sex"].ToString(); ; ;
						user.imagePath = string.IsNullOrEmpty(record["ImagePath"].ToString()) ? string.Empty : record["ImagePath"].ToString(); ; ; ;
						user.FriendRequestStatus = string.IsNullOrEmpty(record["FriendRequestStatus"].ToString()) ? string.Empty : record["FriendRequestStatus"].ToString();
						user.FriendRequestID = Convert.ToInt32(record["FriendRequestID"]);
						user.sessionToken = string.Empty;
						user.mobileNo = string.IsNullOrEmpty(record["MobileNo"].ToString()) ? string.Empty : record["MobileNo"].ToString();
						user.emailID = string.IsNullOrEmpty(record["EmailID"].ToString()) ? string.Empty : record["EmailID"].ToString();
						user.dob = string.IsNullOrEmpty(record["DOB"].ToString()) ? string.Empty : record["DOB"].ToString();
						user.deviceId = string.Empty;

						usersList.Add(user);
					}
				}
				else if (reply.replyCode == 200 && ds.Tables[1].Rows.Count == 0)
				{
					reply.replyCode = Convert.ToInt32(EnumReply.noDataFound);
					reply.replyMessage = "No Data Found";
				}

				searchResult.users = usersList;
			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message;
			}

			searchResult.reply = reply;
			return searchResult;
		}

		[HttpPost]
		public AudioMessagesWrapper GetAudioMessages(Friend friend)
		{
			AudioMessagesWrapper audioMessagesWrapper = new AudioMessagesWrapper();
			List<AudioMessage> audioMessages = new List<AudioMessage>();
			Reply reply = new Reply();
			long fileSize = 0;

			try
			{
				SqlConnection con = new SqlConnection();
				con.ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString;

				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Connection = con;
				cmd.CommandText = "StickMan_usp_GetAudioFileMessages";
				cmd.Parameters.Add("@UserID", System.Data.SqlDbType.Int, 32).Value = friend.UserId;
				cmd.Parameters.Add("@SessionToken", System.Data.SqlDbType.VarChar).Value = friend.SessionToken;
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				adp.Fill(ds);

				reply.replyCode = Convert.ToInt32(ds.Tables[1].Rows[0]["ResponseCode"]);
				reply.replyMessage = ds.Tables[1].Rows[0]["ResponseMesssage"].ToString();

				if (reply.replyCode == 200 && ds.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow record in ds.Tables[0].Rows)
					{
						AudioMessage audioMessage = new AudioMessage();
						audioMessage.message = record["AudioFilePath"].ToString();
						audioMessage.filter = record["Filter"].ToString();

						FileInfo fileinfo = null;
							
						//= new FileInfo(record["AudioFilePath"].ToString());

						if (!string.IsNullOrEmpty("~/Content/Audio/" + record["AudioFilePath"].ToString()) 
							&& File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Audio/" + record["AudioFilePath"].ToString())))
						{
							fileSize = new FileInfo(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Audio/" + record["AudioFilePath"].ToString())).Length;
						}

						audioMessage.fileSize = fileSize;
						audioMessage.time = record["UploadTime"].ToString();
                        audioMessage.SenderId = Convert.ToInt32(record["UserID"]);
                        audioMessage.MessageType = record["MessageType"].ToString();
						audioMessage.user = new User()
						{
							userID = record["MessageType"].ToString() == "Sent" ? Convert.ToInt32(record["RecieverID"]) : Convert.ToInt32(record["UserID"]),
							username = record["UserName"].ToString(),
							sessionToken = friend.SessionToken,
							fullName = record["FullName"].ToString(),
							mobileNo = record["MobileNo"].ToString(),
							emailID = record["EmailID"].ToString(),
							dob = record["DOB"].ToString(),
							sex = record["Sex"].ToString(),
							imagePath = record["ImagePath"].ToString(),
							deviceId = record["DeviceId"].ToString()
						};

						audioMessages.Add(audioMessage);
					}

					audioMessagesWrapper.audioMessages = audioMessages;
				}
				else if (reply.replyCode == 200 && ds.Tables[0].Rows.Count == 0)
				{
					reply.replyCode = Convert.ToInt32(EnumReply.noDataFound);
					reply.replyMessage = "No Data Found";
				}

			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message;
			}

			audioMessagesWrapper.reply = reply;


			return audioMessagesWrapper;
		}

		#region push
		//Push sharp
		public string PushSharpNotification(string deviceID, string message)
		{

			//Create our push services broker
			var push = new PushBroker();

			//Wire up the events for all the services that the broker registers
			push.OnNotificationSent += NotificationSent;
			push.OnChannelException += ChannelException;
			push.OnServiceException += ServiceException;
			push.OnNotificationFailed += NotificationFailed;
			push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
			//push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
			push.OnChannelCreated += ChannelCreated;
			push.OnChannelDestroyed += ChannelDestroyed;


			//-------------------------
			// APPLE NOTIFICATIONS
			//-------------------------
			//Configure and start Apple APNS
			// IMPORTANT: Make sure you use the right Push certificate.  Apple allows you to generate one for connecting to Sandbox,
			//   and one for connecting to Production.  You must use the right one, to match the provisioning profile you build your
			//   app with!
			//var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources/CertificatesNew.p12"));

            //var appleCert = System.Web.Hosting.HostingEnvironment.MapPath("~/utilities/Certificates_dis_final.p12");

            var appleCert = System.Web.Hosting.HostingEnvironment.MapPath("~/utilities/CertificatesKeshP2.p12");

			//IMPORTANT: If you are using a Development provisioning Profile, you must use the Sandbox push notification server 
			//  (so you would leave the first arg in the ctor of ApplePushChannelSettings as 'false')
			//  If you are using an AdHoc or AppStore provisioning profile, you must use the Production push notification server
			//  (so you would change the first arg in the ctor of ApplePushChannelSettings to 'true')
			push.RegisterAppleService(new ApplePushChannelSettings(appleCert,"123123",false)); //Extension method
			//Fluent construction of an iOS notification
			//IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets generated within your iOS app itself when the Application Delegate
			//  for registered for remote notifications is called, and the device token is passed back to you
			push.QueueNotification(new AppleNotification()
									   .ForDeviceToken(deviceID)
									   .WithAlert(message)
									   .WithBadge(7)
									   .WithSound("sound.caf"));

			//Console.WriteLine("Waiting for Queue to Finish...");

			//Stop and wait for the queues to drains
			push.StopAllServices(waitForQueuesToFinish:true);

			return "sucess";

			//Console.WriteLine("Queue Finished, press return to exit...");
			//Console.ReadLine();
		}

		static string DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
		{
			string str = String.Concat("Device Registration Changed:  Old-> ", oldSubscriptionId, "  New-> ", newSubscriptionId, " -> ", notification);

			return str;
			//Currently this event will only ever happen for Android GCM
			//Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
		}

		static void NotificationSent(object sender, INotification notification)
		{
			Console.WriteLine("Sent: " + sender + " -> " + notification);
		}

		static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
		{
			Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
		}

		static void ChannelException(object sender, IPushChannel channel, Exception exception)
		{
			Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
		}

		static void ServiceException(object sender, Exception exception)
		{
			Console.WriteLine("Service Exception: " + sender + " -> " + exception);
		}

		static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
		{
			Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
		}

		static void ChannelDestroyed(object sender)
		{
			Console.WriteLine("Channel Destroyed for: " + sender);
		}

		static void ChannelCreated(object sender, IPushChannel pushChannel)
		{
			Console.WriteLine("Channel Created for: " + sender);
		}
		#endregion push
	}
}