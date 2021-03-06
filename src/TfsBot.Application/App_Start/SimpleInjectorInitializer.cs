[assembly: WebActivator.PostApplicationStartMethod(typeof(TfsBot.App_Start.SimpleInjectorInitializer), "Initialize")]

namespace TfsBot.App_Start
{
    using System.Reflection;
    using System.Web.Mvc;

    using System.Web.Http;
    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;
    using SimpleInjector.Integration.Web;
    using SimpleInjector.Integration.Web.Mvc;
    using Common.Db;

    public static class SimpleInjectorInitializer
    {
        /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
        public static Container Initialize()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            
            InitializeContainer(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();
            
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);

            return container;
        }
     
        private static void InitializeContainer(Container container)
        {
			var configuration = new Configuration();
			container.RegisterSingleton(configuration);
            if (configuration.MongoConnectionString != null)
            {
                container.Register<IRepository>(() => new MongoRepository(configuration.MongoConnectionString, configuration.MongoDbName), Lifestyle.Scoped);
            }
            else if (configuration.StorageConnectionString != null)
			{
				container.Register<IRepository>(() => new Repository(configuration.StorageConnectionString), Lifestyle.Scoped);
			}
			else
			{
				container.Register<IRepository, InMemoryRepository>(Lifestyle.Singleton);
			}
        }
    }
}