using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BabySittingCoop.Domain;
using BabySittingCoop.Domain.Views;
using BabySittingCoop.Migrations;
using BabySittingCoop.Web.Plumbing;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Context;

namespace BabySittingCoop.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static IWindsorContainer Container;
        public static ISessionFactory SessionFactory;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            BootstrapContainer();
        }

        protected void Application_OnEnd()
        {
            //dispose Castle container and all the stuff it contains
            Container.Dispose();
        }

        protected void Application_Error()
        {
            Exception lastException = Server.GetLastError();
        }

        public static ISession CurrentSession
        {
            get { return (ISession)HttpContext.Current.Items["current.session"]; }
            set { HttpContext.Current.Items["current.session"] = value; }
        }

        public ILogger Logger
        {
            get
            {
                return Container.Resolve<ILogger>();
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var app = sender as MvcApplication;
            if (app != null)
            {
                // valid extensions to start a request would be everything except (.css, .js, .htm), 
                if (!Regex.IsMatch(app.Request.FilePath, ".png|.gif|.css|.js|.htm$"))
                {
                    //start new NHibernate session on each web request
                    var session = SessionFactory.OpenSession();

                    CurrentSession = session;
                    //bind session to the thread so all the code can access it using SessionFactory.GetCurrentSession()
                    CurrentSessionContext.Bind(session);
                }
            }

        }

        protected void Application_EndRequest()
        {
            //unbind from the thread
            //no need to close the session as it is already automatically closed at this point (not sure why)
            CurrentSessionContext.Unbind(SessionFactory);

            if (CurrentSession != null)
                CurrentSession.Dispose();

            var context = new HttpContextWrapper(Context);
            if (context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
                context.Response.RedirectLocation = string.Empty;
        }

        private static void BootstrapContainer()
        {
            Container = new WindsorContainer()
                .Install(FromAssembly.This());

            var controllerFactory = new WindsorControllerFactory(Container);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

            var mappingConfiguration = new ConventionConfiguration();
            var config =
                new NHibernate.Cfg.Configuration().Configure(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                                          "NHibernate.config"));
            var autoPersistenceModel =
                AutoMap.AssemblyOf<BabySitter>(mappingConfiguration)
                    .Conventions.Setup(c =>
                                           {
                                               c.Add<PrimaryKeyConvention>();
                                               c.Add<CustomForeignKeyConvention>();
                                               c.Add(DefaultCascade.All());
                                           })
                    .Override<BabySittingPointsView>(map =>
                                                         {
                                                             map.Table("vw_BabySittingPoints");
                                                             map.ReadOnly();
                                                             map.Id(x => x.Id, "BabySitterId");
                                                         });                    

            SessionFactory = Fluently.Configure(config)
                .Mappings(m =>
                {
                    m.AutoMappings.Add(autoPersistenceModel);
                    m.HbmMappings.AddFromAssemblyOf<MvcApplication>();
                })

                .BuildSessionFactory();

            
            Container.Register(Component.For<ISession>().UsingFactoryMethod(() => SessionFactory.GetCurrentSession()).LifestylePerWebRequest());
            //Container.Register(Component.For<ILogger>().ImplementedBy<NLogLogger>());
        }
    }
}