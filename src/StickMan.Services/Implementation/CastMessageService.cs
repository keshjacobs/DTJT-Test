using System;
using System.Collections.Generic;
using System.Linq;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Models;
using StickMan.Services.Models.Message;

namespace StickMan.Services.Implementation
{
	public class CastMessageService : ICastMessageService
	{
		private readonly IUnitOfWork _unitOfWork;

		public CastMessageService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public void Save(string filePath, int userId, string title)
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
		}

		public int ReadMessage(int castMessageId)
		{
			var castMessage = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().GetSingle(m => m.Id == castMessageId);

			castMessage.ClickCount++;
			_unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>().Update(castMessage);
			_unitOfWork.Save();

			return castMessage.ClickCount.GetValueOrDefault();
		}

		public IEnumerable<CastMessage> GetMessages(int page, int size)
		{
			var messages = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>()
				.GetQueryAll()
				.OrderByDescending(u => u.UploadTime)
				.Skip(page * size)
				.Take(size)
				.ToList();

			var userIds = messages.Select(i => i.UserID).Distinct();
			var users = _unitOfWork.Repository<StickMan_Users>().Get(u => userIds.Any(m => m == u.UserID)).ToList();
			var castMessages = GetMergedMessagesInfo(messages, users);

			return castMessages;
		}

		public IEnumerable<CastMessage> Search(string term)
		{
			var users = _unitOfWork.Repository<StickMan_Users>()
				.Get(u => u.UserName.Contains(term) || u.FullName.Contains(term))
				.ToList();
			var userIds = users.Select(u => u.UserID).ToList();

			var messages = _unitOfWork.Repository<StickMan_Users_Cast_AudioData_UploadInformation>()
				.GetQuery(m => m.UserID != null && userIds.Contains(m.UserID.Value))
				.OrderByDescending(m => m.ClickCount)
				.ToList();

			var castMessages = GetMergedMessagesInfo(messages, users);

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

		private IEnumerable<CastMessage> GetMergedMessagesInfo(IEnumerable<StickMan_Users_Cast_AudioData_UploadInformation> messages, ICollection<StickMan_Users> users)
		{
			var castMessages = new List<CastMessage>();

			foreach (var uploadInfo in messages)
			{
				var user = users.FirstOrDefault(u => u.UserID == uploadInfo.UserID);

				var message = CreateCastMessage(uploadInfo);
				FillUserInfo(user, message);

				castMessages.Add(message);
			}

			return castMessages;
		}

		private static CastMessage CreateCastMessage(StickMan_Users_Cast_AudioData_UploadInformation uploadInfo)
		{
			var message = new CastMessage
			{
				MessageInfo = new CastAudioInfo
				{
					Id = uploadInfo.Id,
					AudioFilePath = uploadInfo.AudioFilePath,
					UploadTime = uploadInfo.UploadTime.GetValueOrDefault(),
					Clicks = uploadInfo.ClickCount.GetValueOrDefault(),
					Title = uploadInfo.Title
				}
			};
			return message;
		}

		private static void FillUserInfo(StickMan_Users user, CastMessage message)
		{
			if (user != null)
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
}
