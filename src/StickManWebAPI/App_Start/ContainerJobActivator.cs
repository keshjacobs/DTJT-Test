using Hangfire;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StickManWebAPI.App_Start
{
    public class ContainerJobActivator : JobActivator
    {
        private IUnityContainer _container;

        public ContainerJobActivator(UnityContainer container)
        {
            _container = container;
        }

        public override object ActivateJob(Type type)
        {
            return _container.Resolve(type);
        }
    }
}