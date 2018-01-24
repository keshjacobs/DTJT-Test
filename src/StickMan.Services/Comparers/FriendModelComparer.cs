using System.Collections.Generic;
using StickMan.Services.Models;

namespace StickMan.Services.Comparers
{
	public class FriendModelComparer : IEqualityComparer<FriendModel>
	{
		public bool Equals(FriendModel x, FriendModel y)
		{
			return x.UserId == y.UserId;
		}

		public int GetHashCode(FriendModel obj)
		{
			return obj.UserId.GetHashCode();
		}
	}
}
