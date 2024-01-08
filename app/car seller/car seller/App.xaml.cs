using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Car_Seller
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var shellPage = new ShellPage();
            var navigationPage = new NavigationPage(shellPage);

            MainPage = shellPage;
        }


        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
