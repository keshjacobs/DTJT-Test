using StickManWebAPI.Models.Request;

namespace StickManWebAPI.Models
{
	public class TimelineRequestModel
	{
		public TimelineRequestModel()
		{
			Pagination = new PaginationModel();
		}

		public int UserId { get; set; }

		public string SessionToken { get; set; }

		public PaginationModel Pagination { get; set; }
	}
}