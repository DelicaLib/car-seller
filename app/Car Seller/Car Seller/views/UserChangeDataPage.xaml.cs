using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Car_Seller.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserChangeDataPage : ContentPage
    {
        private MyBasePage m_BasePage;
        string lastPhoneText;
        public UserChangeDataPage()
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

        public static bool ValidatePhone(string phoneNumber)
        {
            return !(phoneNumber == null || phoneNumber.Length != 10 || !long.TryParse(phoneNumber, out long result));
        }

        private void LoginEnrtyChanged(object sender, TextChangedEventArgs e)
        {
            var email = ((Entry)sender).Text;
            EmailErrorLabel.IsVisible = !LoginPage.ValidateEmail(email);
            if (string.IsNullOrWhiteSpace(email))
            {
                EmailErrorLabel.IsVisible = true;
            }
        }
        private async void ChangeButtonClicked(object sender, EventArgs e)
        {
            string email = LoginEnrty.Text;
            if (email != null && !LoginPage.ValidateEmail(email))
            {
                return;
            }
            string phoneNumber = PhoneEnrty.Text;
            if (phoneNumber != null && !ValidatePhone(phoneNumber))
            {
                return;
            }
            if (phoneNumber == null && email == null)
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
                    await ServerInteraction.ChangeUserData(phoneNumber, email, password, recaptchaResponse);
                    await Navigation.PopAsync();
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
                        await RecaptchaWebView.EvaluateJavaScriptAsync("grecaptcha.reset()");
                        RecaptchaErrorLabel.IsVisible = true;
                    }
                    else if (ex.Status == (WebExceptionStatus)401)
                    {
                        await Navigation.PopAsync();
                    }
                    else if (ex.Status == (WebExceptionStatus)403)
                    {
                        await RecaptchaWebView.EvaluateJavaScriptAsync("grecaptcha.reset()");
                        ErrorLabel.IsVisible = true;
                    }
                    return;
                }
            }
        }

        private void PhoneEnrtyChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 0)
            {
                PhoneErrorLabel.IsVisible = false;
                return;
            }
            if (long.TryParse(e.NewTextValue, out long result) && result.ToString() == e.NewTextValue && e.NewTextValue.Length < 11)
            {
                ((Entry)sender).Text = result.ToString();
                lastPhoneText = result.ToString();
            }
            else
            {
                ((Entry)sender).Text = lastPhoneText;
            }
            if (((Entry)sender).Text.Length != 10)
            {
                PhoneErrorLabel.IsVisible = true;
            }
            else
            {
                PhoneErrorLabel.IsVisible = false;
            }
        }
    }
}