using System.ComponentModel.DataAnnotations;

namespace StickManWebAPI.Models.Request
{
	public class PaginationModel
	{
		public PaginationModel()
		{
			PageSize = 30;
			PageNumber = 0;
		}

		[Required]
		[Range(1, int.MaxValue)]
		public int PageSize { get; set; }

		[Required]
		[Range(0, int.MaxValue)]
		public int PageNumber { get; set; }
	}

	public class CastPaginationModel : PaginationModel
	{
		[Required]
		[Range(1, int.MaxValue)]
		public int UserId { get; set; }
	}
}