using Car_Seller.models;
using Car_Seller.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Car_Seller.views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage : ContentPage
	{
        private MyBasePage m_BasePage;
        public ProfilePage ()
		{
            m_BasePage = new MyBasePage(this);
            InitializeComponent ();
		}
        protected override async void OnAppearing()
        {
            if (!await m_BasePage.OnAppearing())
            {
                return;
            }
            base.OnAppearing();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            m_BasePage.OnDisappearing();
        }
    }
}