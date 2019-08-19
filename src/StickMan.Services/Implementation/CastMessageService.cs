using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Extensions;
using StickMan.Services.Models.Message;
using StickMan.Services.Models.User;

namespace StickMan.Services.Implementation
{
	public class CastMessageService : ICastMessageService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAudioFileService _audioFileService;

		public CastMessageService(IUnitOfWork unitOfWork, IAudioFileService audioFileService)
		{
			_unitOfWork = unitOfWork;
			_audioFileService = audioFileService;
		}

		public CastMessage Save(string filePath, int userId, string title, int? replyPostId)
		{
			var message = new StickMan_Users_Cast_AudioData_UploadInformation
			{
				AudioFilePath = filePath,
				UserID = userId,
				ReadStatus = false,
				DeleteStatus = false,
				ClickCount = 0,
				UploadTime = DateTime.UtcNow,
				Title = title,
                ReplyPostId = replyPostId
			};

			_unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().Insert(message);
			_unitOfWork.Save();

			var castMessage = CreateCastMessage(message, userId);
			var user = _unitOfWork.Repository<StickMan_Users>().GetSingle(u => u.UserID == userId);
			FillUserInfo(user, castMessage);

			return castMessage;
		}

		public int ReadMessage(int castMessageId, int currentUserId)
		{
			var castMessage = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetSingle(m => m.Id == castMessageId);

			castMessage.ClickCount++;

			if (castMessage.UsersListened.All(u => u.UserID != currentUserId))
			{
				var currentUser = _unitOfWork.Repository<StickMan_Users>().GetSingle(u => u.UserID == currentUserId);
				currentUser.ListenedCastMessages.Add(castMessage);
				castMessage.UsersListened.Add(currentUser);

				_unitOfWork.Repository<StickMan_Users>().Update(currentUser);
			}
			
			_unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().Update(castMessage);
			_unitOfWork.Save();

			return castMessage.ClickCount.GetValueOrDefault();
		}

		public IEnumerable<CastMessage> GetMessages(int page, int size, int currentUserId,int posId=-1)
		{
			var messages = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>()
				.GetQueryAll()
				.OrderByDescending(u => u.UploadTime)
				.Skip(page * size)
				.Take(size)
				.ToList();
            if (posId != -1 && !messages.Any(c=>c.Id==posId)) {
                var message = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetFirstOrDefault(c => c.Id == posId);
                if(message != null) {
                    messages.Add(message);
                }                    
            }
			var userIds = messages.Select(i => i.UserID).Distinct();
			var users = _unitOfWork.Repository<StickMan_Users>().Get(u => userIds.Any(m => m == u.UserID)).ToList();
			var castMessages = GetMergedMessagesInfo(messages, users, currentUserId);

			return castMessages;
		}

		public IEnumerable<CastMessage> Search(string term, int currentUserId)
		{
			var users = _unitOfWork.Repository<StickMan_Users>()
				.Get(u => u.UserName.Contains(term) || u.FullName.Contains(term))
				.ToList();
			var userIds = users.Select(u => u.UserID).ToList();

			var messages = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>()
				.GetQuery(m => (m.UserID != null && userIds.Contains(m.UserID.Value)) || (m.Title != null && m.Title.Contains(term)))
				.OrderByDescending(m => m.ClickCount)
				.ToList();

			var castMessages = GetMergedMessagesInfo(messages, users, currentUserId);

			return castMessages;
		}

		public string ChangeTitle(int userId, int castId, string newTitle)
		{
			var castMessage = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetSingle(m => m.Id == castId);

			if (castMessage.UserID != userId)
			{
				throw new UnauthorizedAccessException();
			}

			castMessage.Title = newTitle;
			_unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().Update(castMessage);
			_unitOfWork.Save();

			return newTitle;
		}

		private IEnumerable<CastMessage> GetMergedMessagesInfo(
			IEnumerable<StickMan_Users_Cast_AudioData_UploadInformation> messages,
			ICollection<StickMan_Users> users,
			int currentUserId)
		{

            var messageIds = messages.Select(m => m.Id);
			var castMessages = new List<CastMessage>();
            var repostedMessages = _unitOfWork.Repository<PostUser>()
                .GetQueryAll()
                .Where(m => messageIds.Any(a => a == m.PostId) && !m.IsReply)
                .ToList();

            foreach (var uploadInfo in repostedMessages)
            {
                var user = users.FirstOrDefault(u => u.UserID == uploadInfo.UserId);
                var repostedMessage = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetSingle(s => s.Id == uploadInfo.PostId);
                var originalUser = users.FirstOrDefault(u => u.UserID == repostedMessage.UserID);
                var previousDate = repostedMessage.UploadTime;
                repostedMessage.UploadTime = uploadInfo.CreatedAt;
                var message = CreateCastMessage(repostedMessage, uploadInfo.UserId);
                repostedMessage.UploadTime = previousDate;
                if (user != null)
                {
                    FillUserInfo(user, message);
                }
                else
                {
                    user = _unitOfWork.Repository<StickMan_Users>().GetSingle(u => u.UserID == uploadInfo.UserId);
                    FillUserInfo(user, message);
                }

                message.MessageInfo.Type = "repost";
                message.MessageInfo.Description = string.Format("{0} reposted {1}'s post", user.UserName, originalUser.UserName);
                castMessages.Add(message);
            }

            foreach (var uploadInfo in messages)
			{
				var user = users.FirstOrDefault(u => u.UserID == uploadInfo.UserID);

				var message = CreateCastMessage(uploadInfo, currentUserId);
				if (user != null)
				{
					FillUserInfo(user, message);
				}
				else
				{
					user = _unitOfWork.Repository<StickMan_Users>().GetSingle(u => u.UserID == uploadInfo.UserID);
					FillUserInfo(user, message);
				}

                if (uploadInfo.ReplyPostId != null && uploadInfo.ReplyPostId > 0)
                {
                    message.MessageInfo.Type = "reply";
                    var repliedMessage = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetQuery(s => s.Id == uploadInfo.ReplyPostId).FirstOrDefault();
                    if (repliedMessage != null)
                    {
                        var originalUser = users.FirstOrDefault(u => u.UserID == repliedMessage.UserID);
                        if (originalUser != null)
                        {
                            message.MessageInfo.Description = string.Format("{0} replied {1}'s post", user.UserName, originalUser.UserName);
                            message.MessageInfo.OriginalMessageInfo = CreateCastMessage(repliedMessage, originalUser.UserID);
                            FillUserInfo(originalUser, message.MessageInfo.OriginalMessageInfo);
                        }
                    }
                }

                castMessages.Add(message);
			}

            return castMessages.OrderByDescending(u => u.MessageInfo.UploadTime);
        }

		private CastMessage CreateCastMessage(StickMan_Users_Cast_AudioData_UploadInformation uploadInfo, int currentUserId)
		{
            var message = new CastMessage
            {
                MessageInfo = new CastAudioInfo
                {
                    Id = uploadInfo.Id,
                    AudioFilePath = uploadInfo.AudioFilePath,
                    UploadTime = uploadInfo.UploadTime.GetValueOrDefault(),
                    Clicks = uploadInfo.ClickCount.GetValueOrDefault(),
                    Title = uploadInfo.Title,
                    TimePassedSinceUploaded = (DateTime.UtcNow - uploadInfo.UploadTime.GetValueOrDefault()).FormatDuration(),
                    Duration = _audioFileService.GetDuration(uploadInfo.AudioFilePath).FormatDuration(),
                    Likes = uploadInfo.LikePosts.Where(l => l.IsLike).Count(),
                    Dislikes = uploadInfo.LikePosts.Where(l => !l.IsLike).Count(),
                    Reposted = uploadInfo.PostUsers.Where(l => !l.IsReply).Count(),
                    Replied = uploadInfo.PostUsers.Where(l => l.IsReply).Count()
                },
				Listened = uploadInfo.UsersListened.Any(u => u.UserID == currentUserId)
			};
			return message;
		}

		private static void FillUserInfo(StickMan_Users user, CastMessage message)
		{
			message.User = new UserModel
			{
				UserId = user.UserID,
				ImagePath = user.ImagePath,
				UserName = user.UserName,
				DOB = user.DOB,
				DeviceId = user.DeviceId,
				Email = user.EmailID,
				FullName = user.FullName,
				MobileNo = user.MobileNo,
				Sex = user.Sex
			};
		}

        public int LikeMessage(int castMessageId, int userId)
        {
            var castMessage = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetSingle(m => m.Id == castMessageId);
            var like = _unitOfWork.Repository<LikePost>().GetQuery(l => l.PostID == castMessageId && l.UserID == userId).FirstOrDefault();
            if (like == null)
            {
                var newLike = new LikePost
                {
                    PostID = castMessage.Id,
                    UserID = userId,
                    IsLike = true
                };

                _unitOfWork.Repository<LikePost>().Insert(newLike);
                _unitOfWork.Save();
            }
            else
            {
                like.IsLike = true;

                _unitOfWork.Repository<LikePost>().Update(like);
                _unitOfWork.Save();
            }

            var likes = _unitOfWork.Repository<LikePost>().GetQuery(m => m.PostID == castMessageId).ToList();
            var dislikeCount = likes.Count(l => !l.IsLike);
            var totalLikes = likes.Count();
            var percentage = ((double)dislikeCount / totalLikes) * 100;
            if (percentage < 70)
            {
                RecurringJob.RemoveIfExists("delete-post");
            }

            return castMessage.LikePosts.Where(l => l.IsLike).Count();
        }

        public int DislikeMessage(int castMessageId, int userId)
        {
            var castMessage = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetSingle(m => m.Id == castMessageId);
            var like = _unitOfWork.Repository<LikePost>().GetQuery(l => l.PostID == castMessageId && l.UserID == userId).FirstOrDefault();
            if (like == null)
            {
                var newLike = new LikePost
                {
                    PostID = castMessage.Id,
                    UserID = userId,
                    IsLike = false
                };

                _unitOfWork.Repository<LikePost>().Insert(newLike);
                _unitOfWork.Save();
            }
            else
            {
                like.IsLike = false;

                _unitOfWork.Repository<LikePost>().Update(like);
                _unitOfWork.Save();
            }

            var likes = _unitOfWork.Repository<LikePost>().GetQuery(m => m.PostID == castMessageId).ToList();
            var percentage = ((double)likes.Count(l => !l.IsLike) / likes.Count()) * 100;
            if (percentage >= 70)
            {
               RecurringJob.AddOrUpdate("delete-post", () => DeletePost(castMessageId), Cron.Daily);
            }
            else
            {
                RecurringJob.RemoveIfExists("delete-post");
            }

            return castMessage.LikePosts.Where(l => !l.IsLike).Count();
        }

        public CastMessage Repost(int userId, int postId, string comment, int replyId = 0)
        {
            var castMessageId = replyId > 0 ? replyId : postId;
            var postUser = new PostUser
            {
                UserId = userId,
                PostId = castMessageId,
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                IsReply = replyId > 0
            };

            _unitOfWork.Repository<PostUser>().Insert(postUser);
            _unitOfWork.Save();

            var message = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetSingle(u => u.Id == castMessageId);
            var castMessage = CreateCastMessage(message, userId);
            var user = _unitOfWork.Repository<StickMan_Users>().GetSingle(u => u.UserID == userId);
            FillUserInfo(user, castMessage);

            return castMessage;
        }

        public StickMan_Users_Cast_AudioData_UploadInformation GetMessage(int castMessageId)
        {
            var message = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetSingle(u => u.Id == castMessageId);
            return message;
        }

        public void DeletePost(int castMesasgeId)
        {
            var castMessage = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetSingle(m => m.Id == castMesasgeId);
            var postUser = _unitOfWork.Repository<PostUser>().Get(m => m.PostId == castMesasgeId).FirstOrDefault();
            var likePost = _unitOfWork.Repository<LikePost>().Get(m => m.PostID == castMesasgeId).FirstOrDefault();
            if (postUser != null)
            {
                _unitOfWork.Repository<PostUser>().Delete(postUser);
                _unitOfWork.Save();
            }

            if (likePost != null)
            {
                _unitOfWork.Repository<LikePost>().Delete(likePost);
                _unitOfWork.Save();
            }

            if (castMessage != null)
            {
                castMessage.UsersListened.Clear();
                _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().Delete(castMessage);
                _unitOfWork.Save();
            }
        }
    }
}
