using System.Web.Mvc;
using ClassSchedule.Domain.DataAccess;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Web.Controllers;
using Microsoft.Practices.Unity;
using Unity.Mvc5;

namespace ClassSchedule.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();

            // container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<ManageController>(new InjectionConstructor());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}