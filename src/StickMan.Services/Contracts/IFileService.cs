namespace StickMan.Services.Contracts
{
	public interface IFileService
	{
		void SaveFile(int userId, string fileName, string base64Content);
	}
}
