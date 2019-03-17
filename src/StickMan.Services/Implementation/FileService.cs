using System;
using System.IO;
using StickMan.Services.Contracts;

namespace StickMan.Services.Implementation
{
	public class FileService : IFileService
	{
		private readonly IPathProvider _pathProvider;

		public FileService(IPathProvider pathProvider)
		{
			_pathProvider = pathProvider;
		}
		
		public void CopyFile(string sourcePath, string destPath)
        	{
            		if (!Directory.Exists(destPath))
            		{
                		Directory.CreateDirectory(destPath);
            		}
            		File.Copy(sourcePath, destPath, true);
        	}
		
		public void SaveFile(int userId, string fileName, string base64Content)
		{
			var filePath = _pathProvider.BuildAudioPath(fileName);
			var directory = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			File.WriteAllBytes(filePath, Convert.FromBase64String(base64Content));
		}
	}
}
