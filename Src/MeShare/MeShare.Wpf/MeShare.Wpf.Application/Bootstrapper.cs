using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Unity;

namespace MeShare.Wpf.Application
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Views.MainWindow>();
        }

        protected override void InitializeShell()
        {
            System.Windows.Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            //var moduleCatalog = (ModuleCatalog)this.ModuleCatalog;

            //moduleCatalog.AddModule(typeof(UploadModule));


            //Container.RegisterType(typeof(PaymentHandler), "PaymentHandler");

        }
    }
}
