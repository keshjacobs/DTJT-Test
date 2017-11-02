using System.Collections.Generic;
using System.Web.Http;
using StickMan.Services.Contracts;
using StickMan.Services.Models.Message;

namespace StickManWebAPI.Controllers
{
	public class MessageController : ApiController
	{
		private readonly IMessageService _messageService;

		public MessageController(IMessageService messageService)
		{
			_messageService = messageService;
		}

		[HttpGet]
		public IEnumerable<TimelineModel> GetTimeline(int userId)
		{
			var timeline = _messageService.GetTimeline(userId);

			return timeline;
		}
	}
}
