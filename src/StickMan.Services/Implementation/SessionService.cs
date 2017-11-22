using System;
using StickMan.Database;
using StickMan.Database.UnitOfWork;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;

namespace StickMan.Services.Implementation
{
	public class SessionService : ISessionService
	{
		private readonly IUnitOfWork _unitOfWork;

		public SessionService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public void Validate(int userId, string sessionId)
		{
			try
			{
				var session = _unitOfWork.Repository<StickMan_UserSesion>()
					.GetSingle(s => s.UserID == userId && s.SessionID == sessionId);

				if (session == null)
				{
					throw new InvalidSessionException();
				}
			}
			catch (InvalidOperationException)
			{
				throw new InvalidSessionException();
			}
		}
	}
}
