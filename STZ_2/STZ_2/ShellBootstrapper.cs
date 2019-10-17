using System.Windows;
using Caliburn.Micro;
using STZ_2.ViewModels;

namespace STZ_2
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
