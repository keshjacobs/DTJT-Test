using System.IO;
using System.Web.Hosting;
using StickMan.Services.Contracts;

namespace StickMan.Services.Implementation
{
	public class PathProvider : IPathProvider
	{
		public string BuildAudioPath(string fileName)
		{
			if (fileName.StartsWith("/"))
			{
				fileName = fileName.TrimStart('/');
			}

			if (fileName.StartsWith("\\"))
			{
				fileName = fileName.TrimStart('\\');
			}

			var path = HostingEnvironment.MapPath(Path.Combine("~/Content/Audio", fileName));

			return path;
		}
	}
}
