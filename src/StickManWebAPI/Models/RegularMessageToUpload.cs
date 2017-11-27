using System.Collections.Generic;

namespace StickManWebAPI.Models
{
	public class RegularMessageToUpload : MessageToUpload
	{
		public IEnumerable<int> ReceiverIds { get; set; }
	}
}