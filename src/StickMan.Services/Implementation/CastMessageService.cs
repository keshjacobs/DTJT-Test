using System;
using System.Collections.Generic;
using System.Linq;
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

		public CastMessage Save(string filePath, int userId, string title)
		{
			var message = new StickMan_Users_Cast_AudioData_UploadInformation
			{
				AudioFilePath = filePath,
				UserID = userId,
				ReadStatus = false,
				DeleteStatus = false,
				ClickCount = 0,
				UploadTime = DateTime.UtcNow,
				Title = title
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

		public IEnumerable<CastMessage> GetMessages(int page, int size, int currentUserId)
		{
			var messages = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>()
				.GetQueryAll()
				.OrderByDescending(u => u.UploadTime)
				.Skip(page * size)
				.Take(size)
				.ToList();

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
			var castMessages = new List<CastMessage>();

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

				castMessages.Add(message);
			}

			return castMessages;
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
					Duration = _audioFileService.GetDuration(uploadInfo.AudioFilePath).FormatDuration()
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
	}
}
