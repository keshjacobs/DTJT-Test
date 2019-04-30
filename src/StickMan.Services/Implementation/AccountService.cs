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
        private static readonly Encoding Encoding1252 = Encoding.GetEncoding(1252);

        private readonly IUnitOfWork _unitOfWork;

		public AccountService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

        public void CreateFacebookUser(StickMan_Users users) {

        }
        public UserSessionModel FacebookLogin(StickMan_Users userLogin)
        {
            string token;
            //StickMan_Users user = _unitOfWork.Repository<StickMan_Users>().GetFirstOrDefault(u => u.UserName == userLogin.UserName || u.EmailID == userLogin.EmailID);
            StickMan_Users user = _unitOfWork.Repository<StickMan_Users>().GetFirstOrDefault(u => u.EmailID == userLogin.EmailID);
            if (user == null)
            {
                _unitOfWork.Repository<StickMan_Users>().Insert(userLogin);
                user = userLogin;
            }
            else {
                UpdateDeviceId(userLogin.DeviceId, user);
                CleanUpUsersWithTheSameDeviceId(userLogin.DeviceId, user.UserName);
            }
            //if (user != null)
            //{
            //    if (user.UserName.Equals(userLogin.UserName) && ("FACEBOOK").Equals(user.Password))
            //    {
            //        UpdateDeviceId(userLogin.DeviceId, user);
            //        CleanUpUsersWithTheSameDeviceId(userLogin.DeviceId, user.UserName);                    
            //    }
            //    else {
            //        throw new AuthenticationException($"User with email {user.EmailID} already exist.");
            //    }
            //}
            //else {
            //    _unitOfWork.Repository<StickMan_Users>().Insert(userLogin);
            //    user = userLogin;
            //}
            token = CreateSession(user.UserID);

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

        public ResetPasswordModel ResetPassword(int userId)
        {
            var user = GetUser(userId);

            //reset password
            var randomStr = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            var newPassword = EcnryptPassword(randomStr);
            user.Password = newPassword;

            _unitOfWork.Repository<StickMan_Users>().Update(user);
            _unitOfWork.Save();

            var token = CreateSession(user.UserID);

            var userModel = new ResetPasswordModel
            {
                SessionToken = token,
                NewPassword = randomStr
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

        private StickMan_Users GetUser(int userId)
        {
            StickMan_Users user;
            try
            {
                user = _unitOfWork.Repository<StickMan_Users>().GetSingle(u => u.UserID == userId);
            }
            catch (InvalidOperationException)
            {
                throw new AuthenticationException($"User with userId {userId} was not found.");
            }

            return user;
        }

        private static void VerifyPassword(string savedPassword, string enteredPassword)
		{
            var encryptedPass = EcnryptPassword(enteredPassword);

            if (!string.Equals(encryptedPass, savedPassword, StringComparison.CurrentCultureIgnoreCase))
			{
				throw new AuthenticationException("Invalid password");
			}
		}

		private static string EcnryptPassword(string password)
		{
			var passwordData = Encoding1252.GetBytes(password);
            var encryptor = SHA1.Create();
			var computedHash = encryptor.ComputeHash(passwordData);
			var encryptedPass = Encoding1252.GetString(computedHash);

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
