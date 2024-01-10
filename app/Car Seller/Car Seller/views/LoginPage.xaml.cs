using Car_Seller.models;
using Car_Seller.services;
using Car_Seller.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Car_Seller.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private MyBasePage m_BasePage;
        static Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        public LoginPage()
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
            RecaptchaWebView.Source = ServerInteraction.GetURL() + "recaptcha_form";
            base.OnAppearing();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            m_BasePage.OnDisappearing();
        }

        private void LoginEnrtyChanged(object sender, TextChangedEventArgs e)
        {
            var email = ((Entry)sender).Text;
            EmailErrorLabel.IsVisible = !ValidateEmail(email);
            if (string.IsNullOrWhiteSpace(email))
            {
                EmailErrorLabel.IsVisible = true;
            }
        }
        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }

        private async void LoginButtonClicked(object sender, EventArgs e)
        {
            string email = LoginEnrty.Text;
            if (!ValidateEmail(email))
            {
                return;
            }
            string password = PasswordEnrty.Text;
            if (string.IsNullOrWhiteSpace(password))
            {
                return;
            }
            string recaptchaResponse = await RecaptchaWebView.EvaluateJavaScriptAsync("grecaptcha.getResponse()");
            if (recaptchaResponse.Length == 0)
            {
                RecaptchaErrorLabel.IsVisible = true;
            }
            else
            {
                try
                {
                    if (await ServerInteraction.Login(email, password, recaptchaResponse))
                    {
                        ErrorLabel.IsVisible = false;
                        await Shell.Current.Navigation.PopAsync();
                    }
                    else
                    {
                        await RecaptchaWebView.EvaluateJavaScriptAsync("grecaptcha.reset()");
                        ErrorLabel.IsVisible = true;
                    }
                }
                catch (HttpRequestException ex)
                {
                    await m_BasePage.GoToNoServerConnectionPage();
                }
            }
        }
    }
}