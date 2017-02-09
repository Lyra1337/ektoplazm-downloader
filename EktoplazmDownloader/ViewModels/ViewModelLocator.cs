using EktoplazmExtractor.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace EktoplazmExtractor.ViewModels
{
    internal sealed class ViewModelLocator
    {
        public MainWindowViewModel MainWindowVM { get; }

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<HttpTransmissionService>();

            this.MainWindowVM = new MainWindowViewModel();
        }
    }
}