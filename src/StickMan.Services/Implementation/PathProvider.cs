using System.IO;
using System.Web.Hosting;
using StickMan.Services.Contracts;

namespace StickMan.Services.Implementation
{
	public class PathProvider : IPathProvider
	{
		public string BuildAudioPath(string fileName)
		{
			var path = HostingEnvironment.MapPath(Path.Combine("~/Content/Audio/", fileName));

			return path;
		}
	}
}
