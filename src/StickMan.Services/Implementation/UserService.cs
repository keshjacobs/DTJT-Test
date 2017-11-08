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

		public UserModel GetUser(int id)
		{
			var dbUser = _unitOfWork.Repository<StickMan_Users>().GetSingle(u => u.UserID == id);

			return new UserModel
			{
				ImagePath = dbUser.ImagePath,
				UserName = dbUser.UserName,
				UserId = dbUser.UserID,
				FullName = dbUser.FullName,
				DOB = dbUser.DOB,
				MobileNo = dbUser.MobileNo,
				Sex = dbUser.Sex,
				Email = dbUser.EmailID
			};
		}
	}
}
