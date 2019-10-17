using System.Windows;
using Caliburn.Micro;
using STZ_1.ViewModels;

namespace STZ_1
{
    public class ShellBootstrapper : BootstrapperBase
    {
        public ShellBootstrapper() : base(true)
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
