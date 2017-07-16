using System;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Nancy.Bootstrappers.Windsor;
using Nancy.Hosting.Self;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var uri = new Uri("http://localhost:8888");
            using (
                var host =
                    new NancyHost(
                        new HostConfiguration { UrlReservations = new UrlReservations { CreateAutomatically = true } }, uri)
            )
            {
                host.Start();
                Console.WriteLine($@"Your application is running on {uri}");
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }

    public class Bootstrapper : WindsorNancyBootstrapper
    {
        public Bootstrapper()
        {
        }

        protected override void ConfigureApplicationContainer(IWindsorContainer container)
        {
            container.Register(Component.For<IModuleController, ModuleController>().LifestyleTransient());
            base.ConfigureApplicationContainer(container);
        }
    }
}
