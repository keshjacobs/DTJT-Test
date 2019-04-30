namespace StickMan.Services.Contracts
{
	public interface IFileService
	{
		void SaveFile(int userId, string fileName, string base64Content);
        void CopyFile(string sourcePath, string destPath);
    }
}
