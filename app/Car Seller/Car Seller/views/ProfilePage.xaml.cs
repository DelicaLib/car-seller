using Car_Seller.models;
using Car_Seller.services;
using IntelliAbb.Xamarin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        private User userData;
        public ProfilePage()
        {
            m_BasePage = new MyBasePage(this);
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            if (!await m_BasePage.OnAppearing())
            {
                return;
            }
            if (!await ServerInteraction.HasJWTAsync())
            {
                await Shell.Current.GoToAsync("LoginPage");
                return;
            }
            try
            {
                userData = await ServerInteraction.GetProfileData();
            }
            catch (HttpRequestException ex)
            {
                await m_BasePage.GoToNoServerConnectionPage();
                return;
            }
            catch (WebException ex)
            {
                await Shell.Current.GoToAsync("LoginPage");
                return;
            }
            base.OnAppearing();
            SetUserData();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            m_BasePage.OnDisappearing();
        }

        private void SetUserData()
        {
            SurnameLabel.Text = userData.Surname;
            NameLabel.Text = userData.Name;
            EmailLabel.Text = userData.Email;
            StringBuilder phoneNumber = new StringBuilder("+7(");
            for (int i = 0; i < userData.PhoneNumber.Length; i++)
            {
                if (i == 3)
                {
                    phoneNumber.Append(")");
                }
                if (i == 6 || i == 8) 
                {
                    phoneNumber.Append(" ");
                }
                phoneNumber.Append(userData.PhoneNumber[i]);
            }
            PhoneLabel.Text = phoneNumber.ToString();
        }

        private async void LogoutClicked(object sender, EventArgs e)
        {
            try
            {
                await ServerInteraction.Logout();
                await Shell.Current.GoToAsync("//CatalogPage");
            }
            catch (HttpRequestException ex)
            {
                await m_BasePage.GoToNoServerConnectionPage();
                return;
            }
            catch (WebException ex)
            {
                if (ex.Status == (WebExceptionStatus)400)
                {
                    await Navigation.PushModalAsync(new LoginPage());
                }
                return;
            }
        }

        private async void ChangeClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserChangeDataPage());
        }
    }
}