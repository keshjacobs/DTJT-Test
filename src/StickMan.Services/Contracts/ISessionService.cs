namespace StickMan.Services.Contracts
{
	public interface ISessionService
	{
		void Validate(int userId, string sessionId);
	}
}
