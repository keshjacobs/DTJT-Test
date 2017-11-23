using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Exceptions;
using StickManWebAPI.Models;

namespace StickManWebAPI.Controllers
{
	[Obsolete]
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
		public HttpResponseMessage UploadBase64([FromBody]Base64FileContent content)
		{
			if (content == null)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body of the request is required");
			}

			try
			{
				_sessionService.Validate(content.UserId, content.SessionToken);
			}
			catch (InvalidSessionException)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid session");
			}

			var filePath = _pathProvider.BuildAudioPath(Path.Combine(content.UserId.ToString(), content.FileName));
			File.WriteAllBytes(filePath, Convert.FromBase64String(content.Base64Content));

			return Request.CreateResponse(HttpStatusCode.OK, content.FileName);
		}
	}
}
