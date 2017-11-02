using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickManWebAPI.Models;

namespace StickManWebAPI.Controllers
{
	public class AudioFileController : ApiController
	{
		private readonly IPathProvider _pathProvider;
		private readonly ISessionService _sessionService;

		public AudioFileController(IPathProvider pathProvider, ISessionService sessionService)
		{
			_pathProvider = pathProvider;
			_sessionService = sessionService;
		}

		[HttpPost]
		public HttpResponseMessage Upload(FileContent content)
		{
			try
			{
				_sessionService.Validate(content.UserId, content.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session");
			}

			HttpResponseMessage result;
			var httpRequest = HttpContext.Current.Request;
			if (httpRequest.Files.Count > 0)
			{
				var files = new List<string>();
				foreach (string file in httpRequest.Files)
				{
					var postedFile = httpRequest.Files[file];
					var filePath = _pathProvider.BuildAudioPath(content.FileName);
					postedFile.SaveAs(filePath);

					files.Add(filePath);
				}
				result = Request.CreateResponse(HttpStatusCode.Created, files);
			}
			else
			{
				result = Request.CreateResponse(HttpStatusCode.BadRequest, "Files were not found");
			}

			return result;
		}

		[HttpGet]
		public HttpResponseMessage Download([FromUri]FileContent content)
		{
			try
			{
				_sessionService.Validate(content.UserId, content.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session");
			}

			var filePath = _pathProvider.BuildAudioPath(Path.Combine(content.UserId.ToString(), content.FileName));

			using (var fileStream = File.OpenRead(filePath))
			{
				var fileResult = new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StreamContent(fileStream)
				};

				fileResult.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
				{
					FileName = content.FileName
				};

				fileResult.Content.Headers.ContentType = new MediaTypeHeaderValue("audio/caf");

				return fileResult;
			}
		}
	}
}
