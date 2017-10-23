using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StickManWebAPI.Models
{
	public enum EnumReply
	{
		LoginSuccessfull = 200,
		processOk = 201,
		processFail = 300,
		PasswordIncorrect = 301,
		noDataFound = 302,
		userNameRequired = 303
	}
}