using Car_Seller.services;
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
            DependencyService.Register<DataStore>();
            var shellPage = new ShellPage();
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
