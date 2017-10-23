﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace StickManWebAPI
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{

			// Attribute routing.
			//config.MapHttpAttributeRoutes();


			//config.Routes.MapHttpRoute(
			//    name: "DefaultApi",
			//    routeTemplate: "api/{controller}/{id}",
			//    defaults: new { id = RouteParameter.Optional }
			//);

			config.Routes.MapHttpRoute(
            name: "ApiByAction",
            routeTemplate: "api/{controller}/{action}",
            defaults: new { action = "Get" }
        );
			
		}
	}
}
