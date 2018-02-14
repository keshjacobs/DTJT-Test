using System;
using System.Security.Cryptography;
using System.Text;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickMan.Services.Models.User;

namespace StickMan.Services.Implementation
{
	public class AccountService : IAccountService
	{
		private readonly IUnitOfWork _unitOfWork;

		public AccountService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public UserSessionModel Login(string userName, string password, string deviceId)
		{
			var user = GetUser(userName);

			VerifyPassword(user.Password, password);
			UpdateDeviceId(deviceId, user);
			CleanUpUsersWithTheSameDeviceId(deviceId, userName);
			var token = CreateSession(user.UserID);

			var userModel = new UserSessionModel
			{
				SessionToken = token,
				DeviceId = user.DeviceId,
				UserName = user.UserName,
				UserId = user.UserID,
				FullName = user.FullName,
				DOB = user.DOB,
				Email = user.EmailID,
				ImagePath = user.ImagePath,
				MobileNo = user.MobileNo,
				Sex = user.Sex
			};

			return userModel;
		}

		private string CreateSession(int userId)
		{
			var session = new StickMan_UserSesion
			{
				UserID = userId,
				Active = true,
				LoginTime = DateTime.UtcNow,
				SessionID = Guid.NewGuid().ToString()
			};
			_unitOfWork.Repository<StickMan_UserSesion>().Insert(session);
			_unitOfWork.Save();

			return session.SessionID;
		}

		private void CleanUpUsersWithTheSameDeviceId(string deviceId, string userName)
		{
			var otherUsersWithTheSaveDeviceId = _unitOfWork.Repository<StickMan_Users>().Get(u => u.DeviceId == deviceId && u.UserName != userName);
			foreach (var similarUser in otherUsersWithTheSaveDeviceId)
			{
				similarUser.DeviceId = null;
				_unitOfWork.Repository<StickMan_Users>().Update(similarUser);
			}

			_unitOfWork.Save();
		}

		private void UpdateDeviceId(string deviceId, StickMan_Users user)
		{
			user.DeviceId = deviceId;
			_unitOfWork.Repository<StickMan_Users>().Update(user);
			_unitOfWork.Save();
		}

		private StickMan_Users GetUser(string userName)
		{
			StickMan_Users user;
			try
			{
				user = _unitOfWork.Repository<StickMan_Users>().GetSingle(u => u.UserName == userName);
			}
			catch (InvalidOperationException)
			{
				throw new AuthenticationException($"User with username {userName} was not found.");
			}

			return user;
		}

		private static void VerifyPassword(string savedPassword, string enteredPassword)
		{
			var encryptedPass = EcnryptPassword(enteredPassword);

			if (string.Equals(encryptedPass, savedPassword, StringComparison.CurrentCultureIgnoreCase))
			{
				throw new AuthenticationException("Invalid password");
			}
		}

		private static string EcnryptPassword(string password)
		{
			var passwordData = Encoding.ASCII.GetBytes(password);
			var encryptor = SHA1.Create();
			var computedHash = encryptor.ComputeHash(passwordData);
			var encryptedPass = HexStringFromBytes(computedHash);

			return encryptedPass;
		}

		public static string HexStringFromBytes(byte[] bytes)
		{
			var sb = new StringBuilder();
			foreach (var b in bytes)
			{
				var hex = b.ToString("x2");
				sb.Append(hex);
			}

			return sb.ToString();
		}
	}
}
