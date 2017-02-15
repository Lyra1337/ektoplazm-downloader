using System.Windows;
using EktoplazmExtractor.Services;
using Microsoft.Practices.ServiceLocation;

namespace EktoplazmExtractor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Closing += (s, e) => ServiceLocator.Current.GetInstance<HttpTransmissionService>().Stop();
        }
    }
}
