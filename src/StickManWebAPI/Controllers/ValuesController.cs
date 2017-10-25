using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using StickManWebAPI.Models;
using PushSharp.Apple;
using PushSharp;
using PushSharp.Core;

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
			var userWrapper = new UserWrapper();
			var reply = new Reply();
			var user = new User();

			try
			{
				//required field checks
				if (!string.IsNullOrWhiteSpace(username))
				{
					if (!string.IsNullOrWhiteSpace(emailID))
					{
						if (!string.IsNullOrWhiteSpace(password))
						{
							var con = new SqlConnection
							{
								ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
							};

							var cmd = new SqlCommand
							{
								CommandType = CommandType.StoredProcedure,
								Connection = con,

								CommandText = "StickMan_usp_CreateUpdate_User"
							};
							cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = 0;
							cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 500).Value = username;
							cmd.Parameters.Add("@FullName", SqlDbType.VarChar, 500).Value = fullName;
							cmd.Parameters.Add("@Password", SqlDbType.VarChar, 500).Value = password;
							cmd.Parameters.Add("@MobileNo", SqlDbType.VarChar, 500).Value = mobileNo;
							cmd.Parameters.Add("@EmailID", SqlDbType.VarChar, 500).Value = emailID;
							cmd.Parameters.Add("@DOB", SqlDbType.VarChar, 100).Value = dob;
							cmd.Parameters.Add("@Sex", SqlDbType.VarChar, 500).Value = sex;
							cmd.Parameters.Add("@ImagePath", SqlDbType.VarChar, 1024).Value = imagePath;
							cmd.Parameters.Add("@DeviceId", SqlDbType.VarChar, 1024).Value = deviceId;

							var adp = new SqlDataAdapter(cmd);
							var ds = new DataSet();
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
								var mainDIR = @"~\Content\Audio\" + user.userID;

								if (!Directory.Exists(mainDIR))
								{
									Directory.CreateDirectory(HostingEnvironment.MapPath(mainDIR));
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
			var userWrapper = new UserWrapper();
			var reply = new Reply();
			var user = new User();
			try
			{
				var con = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};

				var cmd = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = con,
					CommandText = "StickMan_usp_Login_User"
				};
				cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 32).Value = username;
				cmd.Parameters.Add("@Password", SqlDbType.VarChar, 1024).Value = password;
				cmd.Parameters.Add("@DeviceId", SqlDbType.VarChar, 1024).Value = deviceId;
				var adp = new SqlDataAdapter(cmd);
				var ds = new DataSet();
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
			var audioWrapper = new AudioWrapper();
			//List<AudioWrapper> audioWrapperList = new List<AudioWrapper>();
			var reply = new Reply();
			var pushInfo = new PushInfo();


			try
			{
				var con = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				var cmd = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = con,
					CommandText = "StickMan_usp_SaveAudioPath"
				};
				foreach (var currentrecieverId in audioContent.recieverId)
				{

					cmd.Parameters.Add("@UserID", SqlDbType.Int, 32).Value = audioContent.userId;
					cmd.Parameters.Add("@RecieverID", SqlDbType.Int, 32).Value = currentrecieverId;
					cmd.Parameters.Add("@FilePath", SqlDbType.VarChar, 2048).Value = audioContent.filePath;
					cmd.Parameters.Add("@Filter", SqlDbType.VarChar, 2048).Value = audioContent.filter;
					cmd.Parameters.Add("@SessionToken", SqlDbType.VarChar, 512).Value = audioContent.sessionToken;
					var adp = new SqlDataAdapter(cmd);
					var ds = new DataSet();
					adp.Fill(ds);
					cmd.Parameters.Clear();
					reply.replyCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]);
					reply.replyMessage = ds.Tables[0].Rows[0]["ResponseMesssage"].ToString();
					pushInfo.pushStatus = string.Empty;
					//push message to reciever
					if (reply.replyCode == 200)
					{
						var deviceID = ds.Tables[0].Rows[0]["DeviceID"].ToString();

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

		[HttpPost]
		public SendFriendRequest SendFriendRequest(Friend friend)
		{
			var response = new SendFriendRequest();
			var reply = new Reply();
			var requestDetails = new FriendRequest();
			var deviceID = string.Empty;
			var user = new User();

			var friendRequest = GetAlreadySentFriendRequests(friend);
			if (friendRequest != null)
			{
				response.FriendRequestDetail = new FriendRequest
				{
					FriendRequestId = friendRequest.FriendRequestID,
					FriendRequestState = friendRequest.FriendRequestStatus
				};
				response.user = friendRequest;
				response.reply = new Reply
				{
					replyCode = 400,
					replyMessage = "Such request already sent"
				};
			}

			try
			{
				var con = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};

				var cmd = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = con,
					CommandText = "StickMan_usp_SendFriendRequest"
				};
				cmd.Parameters.Add("@SenderID", SqlDbType.Int, 32).Value = friend.UserId;
				cmd.Parameters.Add("@RecieverID", SqlDbType.Int, 32).Value = friend.RecieverUserId;
				cmd.Parameters.Add("@SessionToken", SqlDbType.VarChar).Value = friend.SessionToken;
				var adp = new SqlDataAdapter(cmd);
				var ds = new DataSet();
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
					}

					//send push notification
					if (Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]) == 200)
					{
						var message = user.username + " sent you a friend request.";

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
			var searchResult = new SearchResult();
			var reply = new Reply();
			var usersList = new List<UserExtension>();


			try
			{
				var con = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};

				var cmd = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = con,
					CommandText = "StickMan_usp_GetUsersList"
				};
				cmd.Parameters.Add("@SessionToken", SqlDbType.VarChar).Value = searchUser.sessionToken;
				cmd.Parameters.Add("@UserId", SqlDbType.Int, 32).Value = searchUser.userId;
				cmd.Parameters.Add("@SearchKeyword", SqlDbType.VarChar).Value = searchUser.searchKeyword;
				var adp = new SqlDataAdapter(cmd);
				var ds = new DataSet();
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
						var user = new UserExtension
						{
							userID = Convert.ToInt32(record["UserID"]),
							username = string.IsNullOrEmpty(record["UserName"].ToString()) ? string.Empty : record["UserName"].ToString(),
							fullName = string.IsNullOrEmpty(record["FullName"].ToString()) ? string.Empty : record["FullName"].ToString()
						};
						;
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
			var reply = new Reply();

			try
			{
				var con = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};

				var cmd = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = con,
					CommandText = "StickMan_usp_ResponseFriendRequest"
				};
				cmd.Parameters.Add("@UserID", SqlDbType.Int, 32).Value = friend.UserId;
				cmd.Parameters.Add("@RespondingToUserID", SqlDbType.Int, 32).Value = friend.RespondingToUserID;
				cmd.Parameters.Add("@FriendRequestID", SqlDbType.Int, 32).Value = friend.FriendRequestId;
				cmd.Parameters.Add("@FriendRequestReply", SqlDbType.Int, 32).Value = friend.FriendRequestReply;
				cmd.Parameters.Add("@SessionToken", SqlDbType.VarChar).Value = friend.SessionToken;
				var adp = new SqlDataAdapter(cmd);
				var ds = new DataSet();
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
			var searchResult = new SearchResult();
			var reply = new Reply();
			var usersList = new List<UserExtension>();

			try
			{
				var con = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};

				var cmd = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = con,
					CommandText = "StickMan_usp_GetFriends"
				};
				cmd.Parameters.Add("@UserID", SqlDbType.Int, 32).Value = friend.UserId;
				cmd.Parameters.Add("@SessionToken", SqlDbType.VarChar).Value = friend.SessionToken;
				var adp = new SqlDataAdapter(cmd);
				var ds = new DataSet();
				adp.Fill(ds);

				reply.replyCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]);
				reply.replyMessage = ds.Tables[0].Rows[0]["ResponseMesssage"].ToString();

				if (reply.replyCode == 200 && ds.Tables[1].Rows.Count > 0)
				{
					foreach (DataRow record in ds.Tables[1].Rows)
					{
						var user = new UserExtension
						{
							userID = Convert.ToInt32(record["UserID"]),
							username = record["UserName"].ToString(),
							fullName = record["FullName"].ToString(),
							imagePath = record["ImagePath"].ToString(),
							sex = record["Sex"].ToString(),
							mobileNo = record["MobileNo"].ToString(),
							emailID = record["EmailID"].ToString(),
							dob = record["DOB"].ToString()
						};
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
			var searchResult = new SearchResult();
			var reply = new Reply();
			var usersList = new List<UserExtension>();

			try
			{
				var con = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};

				var cmd = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = con,
					CommandText = "StickMan_usp_GetPendingFriendRequests"
				};
				cmd.Parameters.Add("@SessionToken", SqlDbType.VarChar).Value = friend.SessionToken;
				cmd.Parameters.Add("@UserId", SqlDbType.Int, 32).Value = friend.UserId;

				var adp = new SqlDataAdapter(cmd);
				var ds = new DataSet();
				adp.Fill(ds);

				reply.replyCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResponseCode"]);
				reply.replyMessage = ds.Tables[0].Rows[0]["ResponseMesssage"].ToString();

				if (reply.replyCode == 200 && ds.Tables[1].Rows.Count > 0)
				{
					foreach (DataRow record in ds.Tables[1].Rows)
					{
						var user = new UserExtension
						{
							userID = Convert.ToInt32(record["UserID"]),
							username = string.IsNullOrEmpty(record["UserName"].ToString()) ? string.Empty : record["UserName"].ToString(),
							fullName = string.IsNullOrEmpty(record["FullName"].ToString()) ? string.Empty : record["FullName"].ToString(),
							sex = string.IsNullOrEmpty(record["Sex"].ToString()) ? string.Empty : record["Sex"].ToString(),
							imagePath = string.IsNullOrEmpty(record["ImagePath"].ToString()) ? string.Empty : record["ImagePath"].ToString(),
							FriendRequestStatus = string.IsNullOrEmpty(record["FriendRequestStatus"].ToString())
								? string.Empty
								: record["FriendRequestStatus"].ToString(),
							FriendRequestID = Convert.ToInt32(record["FriendRequestID"]),
							sessionToken = string.Empty,
							mobileNo = string.IsNullOrEmpty(record["MobileNo"].ToString()) ? string.Empty : record["MobileNo"].ToString(),
							emailID = string.IsNullOrEmpty(record["EmailID"].ToString()) ? string.Empty : record["EmailID"].ToString(),
							dob = string.IsNullOrEmpty(record["DOB"].ToString()) ? string.Empty : record["DOB"].ToString(),
							deviceId = string.Empty
						};

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
		public SearchResult GetBlockedFriends(Friend friend)
		{
			var searchResult = new SearchResult();
			var reply = new Reply();
			var userExtensionList = new List<UserExtension>();
			try
			{
				var sqlConnection = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				var selectCommand = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = sqlConnection,
					CommandText = "StickMan_usp_GetBlockedFriends"
				};
				selectCommand.Parameters.Add("@UserID", SqlDbType.Int, 32).Value = friend.UserId;
				selectCommand.Parameters.Add("@SessionToken", SqlDbType.VarChar).Value = friend.SessionToken;
				var sqlDataAdapter = new SqlDataAdapter(selectCommand);
				var dataSet = new DataSet();
				sqlDataAdapter.Fill(dataSet);
				reply.replyCode = Convert.ToInt32(dataSet.Tables[0].Rows[0]["ResponseCode"]);
				reply.replyMessage = dataSet.Tables[0].Rows[0]["ResponseMesssage"].ToString();
				if (reply.replyCode == 200 && dataSet.Tables[1].Rows.Count > 0)
				{
					foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[1].Rows)
					{
						var userExtension = new UserExtension
						{
							userID = Convert.ToInt32(row["UserID"]),
							username = row["UserName"].ToString(),
							fullName = row["FullName"].ToString(),
							imagePath = row["ImagePath"].ToString(),
							sex = row["Sex"].ToString(),
							mobileNo = row["MobileNo"].ToString(),
							emailID = row["EmailID"].ToString(),
							dob = row["DOB"].ToString()
						};
						userExtensionList.Add(userExtension);
					}
					searchResult.users = userExtensionList;
				}
				else if (reply.replyCode == 200)
				{
					if (dataSet.Tables[1].Rows.Count == 0)
					{
						reply.replyCode = Convert.ToInt32(EnumReply.noDataFound);
						reply.replyMessage = "No Data Found";
					}
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
		public string DeleteFriendRequest(string UserId, string ReceiverId)
		{
			try
			{
				var sqlConnection = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				sqlConnection.Open();
				var sqlCommand = new SqlCommand
				{
					CommandType = CommandType.Text,
					Connection = sqlConnection
				};
				sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = Convert.ToInt32(UserId);
				sqlCommand.Parameters.Add("@RecieverID", SqlDbType.Int).Value = Convert.ToInt32(ReceiverId);
				sqlCommand.CommandText = "Update StickMan_FriendRequest set FriendRequestStatus=2 Where UserID=@UserID  AND RecieverID=@RecieverID";
				sqlCommand.ExecuteNonQuery();
				return "Success";
			}
			catch
			{
				return "Failed";
			}
		}

		[HttpPost]
		public BlockfrndStatus BlockFriend(BlockfrndStatus values)
		{
			try
			{
				var sqlConnection = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				sqlConnection.Open();
				var sqlCommand = new SqlCommand
				{
					CommandType = CommandType.Text,
					Connection = sqlConnection
				};
				sqlCommand.Parameters.Add("@RecieverID", SqlDbType.Int, 32).Value = values.ReceiverId;
				sqlCommand.Parameters.Add("@UserID", SqlDbType.Int, 32).Value = values.UserId;
				sqlCommand.CommandText = "Update StickMan_FriendRequest set BlockedBy=@UserID Where (UserID=@UserID or RecieverID=@UserID) AND (RecieverID=@RecieverID or UserID=@RecieverID)";
				sqlCommand.ExecuteNonQuery();
				return values;
			}
			catch
			{
				return values;
			}
		}

		[HttpPost]
		public Unblock UnBlockFriend(Unblock values)
		{
			try
			{
				var sqlConnection = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				sqlConnection.Open();
				var sqlCommand = new SqlCommand
				{
					CommandType = CommandType.Text,
					Connection = sqlConnection
				};
				sqlCommand.Parameters.Add("@RecieverID", SqlDbType.Int, 32).Value = values.ReceiverId;
				sqlCommand.Parameters.Add("@UserID", SqlDbType.Int, 32).Value = values.UserId;
				sqlCommand.CommandText = "Update StickMan_FriendRequest set BlockedBy=null Where (UserID=@UserID or RecieverID=@UserID) AND (RecieverID=@RecieverID or UserID=@RecieverID)";
				sqlCommand.ExecuteNonQuery();
				return values;
			}
			catch
			{
				return values;
			}
		}

		[HttpPost]
		public DeletefrndStatus DeleteFriendRequests(DeletefrndStatus values)
		{
			try
			{
				var sqlConnection = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				sqlConnection.Open();
				var sqlCommand = new SqlCommand
				{
					CommandType = CommandType.Text,
					Connection = sqlConnection
				};
				sqlCommand.Parameters.Add("@RecieverID", SqlDbType.Int, 32).Value = values.ReceiverId;
				sqlCommand.Parameters.Add("@UserID", SqlDbType.Int, 32).Value = values.UserId;
				sqlCommand.CommandText = "Update StickMan_FriendRequest set FriendRequestStatus=2 Where (UserID=@UserID or RecieverID=@UserID) AND (RecieverID=@RecieverID or UserID=@RecieverID)";
				sqlCommand.ExecuteNonQuery();
				return values;
			}
			catch
			{
				return values;
			}
		}

		[HttpPost]
		public string CastMessageStatus(ReadStatus readstatus)
		{
			try
			{
				var sqlConnection = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				sqlConnection.Open();
				var sqlCommand = new SqlCommand
				{
					CommandType = CommandType.Text,
					Connection = sqlConnection,
					CommandText = "Update StickMan_Users_Cast_AudioData_UploadInformation set ReadStatus=1 Where RecieverID=@RecieverID AND AudioFilePath=@FilePath"
				};
				sqlCommand.Parameters.AddWithValue("@RecieverID", Convert.ToInt32(readstatus.receiverId));
				sqlCommand.Parameters.AddWithValue("@FilePath", readstatus.file);
				sqlCommand.ExecuteNonQuery();
				return "Success";
			}
			catch
			{
				return "Failed";
			}
		}

		[HttpPost]
		public string RowClickCount(ClickCount clickcount)
		{
			try
			{
				var sqlConnection = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				sqlConnection.Open();
				var sqlCommand = new SqlCommand
				{
					CommandType = CommandType.Text,
					Connection = sqlConnection,
					CommandText = "Update Cast_AudioLog set ClickCount = clickcount + 1 Where AudioFilePath=@FilePath"
				};
				sqlCommand.Parameters.AddWithValue("@FilePath", clickcount.file);
				sqlCommand.ExecuteNonQuery();
				return "Success";
			}
			catch
			{
				return "Failed";
			}
		}

		[HttpPost]
		public string MessageStatus(ReadStatus readstatus)
		{
			try
			{
				var sqlConnection = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				sqlConnection.Open();
				var sqlCommand = new SqlCommand
				{
					CommandType = CommandType.Text,
					Connection = sqlConnection,
					CommandText = "Update StickMan_Users_AudioData_UploadInformation set ReadStatus=1 Where RecieverID=@RecieverID AND AudioFilePath=@FilePath"
				};
				sqlCommand.Parameters.AddWithValue("@RecieverID", Convert.ToInt32(readstatus.receiverId));
				sqlCommand.Parameters.AddWithValue("@FilePath", readstatus.file);
				sqlCommand.ExecuteNonQuery();
				return "Success";
			}
			catch
			{
				return "Failed";
			}
		}

		[HttpPost]
		public AudioMessagesWrapper messagereceipents(Friend friend)
		{
			return new AudioMessagesWrapper();
		}

		[HttpPost]
		public AudioMessagesWrapper NewGetAudioMessages(Friend friend)
		{
			var audioMessagesWrapper = new AudioMessagesWrapper();
			var newAudioMessageList = new List<NewAudioMessage>();
			var reply = new Reply();
			long num = 0;
			try
			{
				var sqlConnection = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				var selectCommand = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = sqlConnection,
					CommandText = "StickMan_usp_NewGetAudioFileMessages"
				};
				selectCommand.Parameters.Add("@UserID", SqlDbType.Int, 32).Value = friend.UserId;
				selectCommand.Parameters.AddWithValue("@SessionToken", SqlDbType.VarChar).Value = friend.SessionToken;
				var sqlDataAdapter = new SqlDataAdapter(selectCommand);
				var dataSet = new DataSet();
				sqlDataAdapter.Fill(dataSet);
				reply.replyCode = Convert.ToInt32(dataSet.Tables[1].Rows[0]["ResponseCode"]);
				reply.replyMessage = dataSet.Tables[1].Rows[0]["ResponseMesssage"].ToString();
				if (reply.replyCode == 200 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
					{
						var newAudioMessage = new NewAudioMessage
						{
							message = row["AudioFilePath"].ToString(),
							filter = row["Filter"].ToString(),
							id = Convert.ToInt32(row["NewAudioID"])
						};
						if (!string.IsNullOrEmpty("~/Content/Audio/" + row["AudioFilePath"]) && File.Exists(HostingEnvironment.MapPath("~/Content/Audio/" + row["AudioFilePath"])))
							num = new FileInfo(HostingEnvironment.MapPath("~/Content/Audio/" + row["AudioFilePath"])).Length;
						newAudioMessage.fileSize = num;
						newAudioMessage.time = row["UploadTime"].ToString();
						newAudioMessage.MessageType = row["MessageType"].ToString();
						newAudioMessage.readstatus = (bool)row["ReadStatus"];
						newAudioMessage.deletestatus = (bool)row["DeleteStatus"];
						newAudioMessage.iscasted = (bool)row["IsCasted"];
						newAudioMessage.user = new User()
						{
							userID = row["MessageType"].ToString() == "Sent" ? Convert.ToInt32(row["UserID"]) : Convert.ToInt32(row["UserID"]),
							username = row["UserName"].ToString(),
							sessionToken = friend.SessionToken,
							fullName = row["FullName"].ToString(),
							mobileNo = row["MobileNo"].ToString(),
							emailID = row["EmailID"].ToString(),
							dob = row["DOB"].ToString(),
							sex = row["Sex"].ToString(),
							imagePath = row["ImagePath"].ToString(),
							deviceId = row["DeviceId"].ToString()
						};
						newAudioMessageList.Add(newAudioMessage);
					}
					audioMessagesWrapper.newaudioMessages = newAudioMessageList;
				}
				else if (reply.replyCode == 200)
				{
					if (dataSet.Tables[0].Rows.Count == 0)
					{
						reply.replyCode = Convert.ToInt32(EnumReply.noDataFound);
						reply.replyMessage = "No Data Foundd";
					}
				}
			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message + friend.UserId;
			}
			audioMessagesWrapper.reply = reply;
			return audioMessagesWrapper;
		}

		[HttpPost]
		public AudioMessagesWrapper GetAudioMessages(Friend friend)
		{
			var audioMessagesWrapper = new AudioMessagesWrapper();
			var audioMessages = new List<AudioMessage>();
			var reply = new Reply();
			long fileSize = 0;

			try
			{
				var con = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};

				var cmd = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = con,
					CommandText = "StickMan_usp_GetAudioFileMessages"
				};
				cmd.Parameters.Add("@UserID", SqlDbType.Int, 32).Value = friend.UserId;
				cmd.Parameters.Add("@SessionToken", SqlDbType.VarChar).Value = friend.SessionToken;
				var adp = new SqlDataAdapter(cmd);
				var ds = new DataSet();
				adp.Fill(ds);

				reply.replyCode = Convert.ToInt32(ds.Tables[1].Rows[0]["ResponseCode"]);
				reply.replyMessage = ds.Tables[1].Rows[0]["ResponseMesssage"].ToString();

				if (reply.replyCode == 200 && ds.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow record in ds.Tables[0].Rows)
					{
						var audioMessage = new AudioMessage
						{
							message = record["AudioFilePath"].ToString(),
							filter = record["Filter"].ToString()
						};

						FileInfo fileinfo = null;

						//= new FileInfo(record["AudioFilePath"].ToString());

						if (!string.IsNullOrEmpty("~/Content/Audio/" + record["AudioFilePath"])
							&& File.Exists(HostingEnvironment.MapPath("~/Content/Audio/" + record["AudioFilePath"])))
						{
							fileSize = new FileInfo(HostingEnvironment.MapPath("~/Content/Audio/" + record["AudioFilePath"])).Length;
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

		[HttpPost]
		public AudioMessagesWrapper CastGetAudioMessages(Friend friend)
		{
			var audioMessagesWrapper = new AudioMessagesWrapper();
			var audioMessageList = new List<AudioMessage>();
			var reply = new Reply();
			long num = 0;
			try
			{
				var sqlConnection = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				var selectCommand = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = sqlConnection,
					CommandText = "StickMan_usp_Cast_GetAudioFileMessages"
				};
				selectCommand.Parameters.Add("@UserID", SqlDbType.Int, 32).Value = friend.UserId;
				selectCommand.Parameters.AddWithValue("@SessionToken", SqlDbType.VarChar).Value = friend.SessionToken;
				var sqlDataAdapter = new SqlDataAdapter(selectCommand);
				var dataSet = new DataSet();
				sqlDataAdapter.Fill(dataSet);
				reply.replyCode = Convert.ToInt32(dataSet.Tables[1].Rows[0]["ResponseCode"]);
				reply.replyMessage = dataSet.Tables[1].Rows[0]["ResponseMesssage"].ToString();
				if (reply.replyCode == 200 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
					{
						var audioMessage = new AudioMessage
						{
							message = row["AudioFilePath"].ToString(),
							filter = row["Filter"].ToString()
						};
						if (!string.IsNullOrEmpty("~/Content/Audio/" + row["AudioFilePath"]) && File.Exists(HostingEnvironment.MapPath("~/Content/Audio/" + row["AudioFilePath"])))
							num = new FileInfo(HostingEnvironment.MapPath("~/Content/Audio/" + row["AudioFilePath"])).Length;
						audioMessage.fileSize = num;
						audioMessage.time = row["UploadTime"].ToString();
						audioMessage.SenderId = Convert.ToInt32(row["UserID"]);
						audioMessage.MessageType = row["MessageType"].ToString();
						audioMessage.readstatus = (bool)row["ReadStatus"];
						audioMessage.deletestatus = (bool)row["DeleteStatus"];
						audioMessage.clickcount = Convert.ToInt32(row["ClickCount"]);
						audioMessage.user = new User()
						{
							userID = row["MessageType"].ToString() == "Sent" ? Convert.ToInt32(row["RecieverID"]) : Convert.ToInt32(row["UserID"]),
							username = row["UserName"].ToString(),
							sessionToken = friend.SessionToken,
							fullName = row["FullName"].ToString(),
							mobileNo = row["MobileNo"].ToString(),
							emailID = row["EmailID"].ToString(),
							dob = row["DOB"].ToString(),
							sex = row["Sex"].ToString(),
							imagePath = row["ImagePath"].ToString(),
							deviceId = row["DeviceId"].ToString()
						};
						audioMessageList.Add(audioMessage);
					}
					audioMessagesWrapper.audioMessages = audioMessageList;
				}
				else if (reply.replyCode == 200)
				{
					if (dataSet.Tables[0].Rows.Count == 0)
					{
						reply.replyCode = Convert.ToInt32(EnumReply.noDataFound);
						reply.replyMessage = "No Data Foundd";
					}
				}
			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message + friend.UserId;
			}
			audioMessagesWrapper.reply = reply;
			return audioMessagesWrapper;
		}

		[HttpPost]
		public AudioMessagesWrapper NewCastGetAudioMessages(Friend friend)
		{
			var audioMessagesWrapper = new AudioMessagesWrapper();
			var newAudioMessageList = new List<NewAudioMessage>();
			var reply = new Reply();
			long num = 0;
			try
			{
				var sqlConnection = new SqlConnection
				{
					ConnectionString = ConfigurationManager.ConnectionStrings["StickManConnection"].ConnectionString
				};
				var selectCommand = new SqlCommand
				{
					CommandType = CommandType.StoredProcedure,
					Connection = sqlConnection,
					CommandText = "StickMan_usp_Cast_NewGetAudioFileMessages"
				};
				selectCommand.Parameters.Add("@UserID", SqlDbType.Int, 32).Value = friend.UserId;
				selectCommand.Parameters.AddWithValue("@SessionToken", SqlDbType.VarChar).Value = friend.SessionToken;
				var sqlDataAdapter = new SqlDataAdapter(selectCommand);
				var dataSet = new DataSet();
				sqlDataAdapter.Fill(dataSet);
				reply.replyCode = Convert.ToInt32(dataSet.Tables[1].Rows[0]["ResponseCode"]);
				reply.replyMessage = dataSet.Tables[1].Rows[0]["ResponseMesssage"].ToString();
				if (reply.replyCode == 200 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
					{
						var newAudioMessage = new NewAudioMessage
						{
							message = row["AudioFilePath"].ToString(),
							filter = row["Filter"].ToString()
						};
						if (!string.IsNullOrEmpty("~/Content/Audio/" + row["AudioFilePath"]) && File.Exists(HostingEnvironment.MapPath("~/Content/Audio/" + row["AudioFilePath"])))
							num = new FileInfo(HostingEnvironment.MapPath("~/Content/Audio/" + row["AudioFilePath"])).Length;
						newAudioMessage.fileSize = num;
						newAudioMessage.MessageType = row["MessageType"].ToString();
						newAudioMessage.readstatus = (bool)row["ReadStatus"];
						newAudioMessage.deletestatus = (bool)row["DeleteStatus"];
						newAudioMessage.iscasted = (bool)row["IsCasted"];
						newAudioMessage.clickcount = Convert.ToInt32(row["ClickCount"]);
						newAudioMessage.user = new User()
						{
							userID = Convert.ToInt32(row["UserID"]),
							username = row["UserName"].ToString(),
							sessionToken = friend.SessionToken,
							fullName = row["FullName"].ToString(),
							mobileNo = row["MobileNo"].ToString(),
							emailID = row["EmailID"].ToString(),
							dob = row["DOB"].ToString(),
							sex = row["Sex"].ToString(),
							deviceId = row["DeviceId"].ToString()
						};
						newAudioMessageList.Add(newAudioMessage);
					}
					audioMessagesWrapper.newaudioMessages = newAudioMessageList;
				}
				else if (reply.replyCode == 200)
				{
					if (dataSet.Tables[0].Rows.Count == 0)
					{
						reply.replyCode = Convert.ToInt32(EnumReply.noDataFound);
						reply.replyMessage = "No Data Foundd";
					}
				}
			}
			catch (Exception ex)
			{
				reply.replyCode = Convert.ToInt32(EnumReply.processFail);
				reply.replyMessage = ex.Message + friend.UserId;
			}
			audioMessagesWrapper.reply = reply;
			return audioMessagesWrapper;
		}

		private UserExtension GetAlreadySentFriendRequests(Friend friend)
		{
			var requests = GetPendingFriendRequests(friend);
			var friendRequest = requests.users.FirstOrDefault(x => x.FriendRequestID == friend.FriendRequestId || x.userID == friend.RecieverUserId);

			return friendRequest;
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

			var appleCert = HostingEnvironment.MapPath("~/utilities/CertificatesKeshP2.p12");

			//IMPORTANT: If you are using a Development provisioning Profile, you must use the Sandbox push notification server 
			//  (so you would leave the first arg in the ctor of ApplePushChannelSettings as 'false')
			//  If you are using an AdHoc or AppStore provisioning profile, you must use the Production push notification server
			//  (so you would change the first arg in the ctor of ApplePushChannelSettings to 'true')
			push.RegisterAppleService(new ApplePushChannelSettings(appleCert, "123123", false)); //Extension method
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
			push.StopAllServices(waitForQueuesToFinish: true);

			return "sucess";

			//Console.WriteLine("Queue Finished, press return to exit...");
			//Console.ReadLine();
		}

		static string DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
		{
			var str = String.Concat("Device Registration Changed:  Old-> ", oldSubscriptionId, "  New-> ", newSubscriptionId, " -> ", notification);

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