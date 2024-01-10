using Car_Seller.models;
using Car_Seller.services;
using Car_Seller.views;
using System;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Car_Seller
{
    public partial class App : Application
    {

        private static CookieDB cookieDBObj;

        public static CookieDB CookieDBObj
        {
            get
            {
                if (cookieDBObj == null)
                {
                    cookieDBObj = new CookieDB(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "myCookie.db3"));
                }
                return cookieDBObj;
            }
        }

        public App()
        {
            InitializeComponent();
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("CarPage", typeof(CarPage));
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
