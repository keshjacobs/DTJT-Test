using System.Collections.Generic;
using System.Linq;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Models;

namespace StickMan.Services.Implementation
{
	public class UserService : IUserService
	{
		private readonly IUnitOfWork _unitOfWork;

		public UserService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IEnumerable<UserModel> GetUsers(IEnumerable<int> ids)
		{
			var users = _unitOfWork.Repository<StickMan_Users>().Get(u => ids.Contains(u.UserID));

			return users.Select(MapUserModels);
		}

		public UserModel GetUser(int id)
		{
			var dbUser = _unitOfWork.Repository<StickMan_Users>().GetSingle(u => u.UserID == id);

			return MapUserModels(dbUser);
		}

		private static UserModel MapUserModels(StickMan_Users dbUser)
		{
			return new UserModel
			{
				ImagePath = dbUser.ImagePath,
				UserName = dbUser.UserName,
				UserId = dbUser.UserID,
				FullName = dbUser.FullName,
				DOB = dbUser.DOB,
				MobileNo = dbUser.MobileNo,
				Sex = dbUser.Sex,
				Email = dbUser.EmailID,
			    DeviceId = dbUser.DeviceId
			};
		}
	}
}
