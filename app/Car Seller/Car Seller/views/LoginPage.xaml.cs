using Car_Seller.models;
using Car_Seller.services;
using Car_Seller.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private string lastPhoneText;
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

        private bool PassWordValidate()
        {
            if (PasswordEnrty.Text.Length < 8)
            {
                PasswordErrorLabel.Text = "Пароль должен состоять минимум из 8 символов";
                PasswordErrorLabel.IsVisible = true;
                return false;
            }
            Regex Lowercase = new Regex("[a-z]");
            Regex Uppercase = new Regex("[A-Z]");
            Regex Digit = new Regex("[0-9]");
            Regex Special = new Regex(@"[\W_]");
            if (!Lowercase.IsMatch(PasswordEnrty.Text))
            {
                PasswordErrorLabel.Text = "Пароль должен содержать строчные латинские буквы";
                PasswordErrorLabel.IsVisible = true;
                return false;
            }
            if (!Uppercase.IsMatch(PasswordEnrty.Text))
            {
                PasswordErrorLabel.Text = "Пароль должен содержать заглавные латинские буквы";
                PasswordErrorLabel.IsVisible = true;
                return false;
            }
            if (!Digit.IsMatch(PasswordEnrty.Text))
            {
                PasswordErrorLabel.Text = "Пароль должен содержать цифры";
                PasswordErrorLabel.IsVisible = true;
                return false;
            }
            if (!Special.IsMatch(PasswordEnrty.Text))
            {
                PasswordErrorLabel.Text = "Пароль должен содержать специальные символы";
                PasswordErrorLabel.IsVisible = true;
                return false;
            }
            PasswordErrorLabel.IsVisible = false;
            return true;
        }

        private void SetRegisterPage()
        {
            
            RecaptchaWebView.Reload();
            RecaptchaWebView.HeightRequest = 500;
            Title = "Регистрация";
            PhoneLayout.IsVisible = true;
            PasswordEnrty.TextChanged += PasswordTextChanged;
            PasswordRepeatLayout.IsVisible = true;
            NameLayout.IsVisible = true;
            SurnameLayout.IsVisible = true;
            ErrorLabel.Text = "Неправильный логин и/или пароль";
        }
        private void SetLoginPage()
        {
            RecaptchaWebView.Reload();
            RecaptchaWebView.HeightRequest = 500;
            Title = "Авторизация";
            PhoneLayout.IsVisible = false;
            PasswordEnrty.TextChanged -= PasswordTextChanged;
            PasswordRepeatLayout.IsVisible = false;
            NameLayout.IsVisible = false;
            SurnameLayout.IsVisible = false;
        }
        private async void LoginButtonClicked(object sender, EventArgs e)
        {
            if (Title == "Регистрация")
            {
                SetLoginPage();

                return;
            }
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



        private async void RegisterButtonClicked(object sender, EventArgs e)
        {
            if (Title == "Авторизация")
            {
                SetRegisterPage();
                
                return;
            }
            ErrorLabel.IsVisible = false;
            RecaptchaErrorLabel.IsVisible = false;
            string name = NameEnrty.Text;
            if (name == null)
            {
                return;
            }
            string surname = SurnameEnrty.Text;
            if (surname == null)
            {
                return;
            }
            string email = LoginEnrty.Text;
            if (!ValidateEmail(email))
            {
                return;
            }
            string phoneNumber = PhoneEnrty.Text;
            if (!UserChangeDataPage.ValidatePhone(phoneNumber))
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
                    await ServerInteraction.Register(name, surname, email, phoneNumber, password, recaptchaResponse);
                    ErrorLabel.IsVisible = false;
                    SetLoginPage();
                }
                catch (HttpRequestException ex)
                {
                    await RecaptchaWebView.EvaluateJavaScriptAsync("grecaptcha.reset()");
                    await m_BasePage.GoToNoServerConnectionPage();
                }
                catch (WebException ex)
                {
                    await RecaptchaWebView.EvaluateJavaScriptAsync("grecaptcha.reset()");
                    if (ex.Status == (WebExceptionStatus)403)
                    {
                        ErrorLabel.Text = "Такой телефон и/или почта заняты";
                        ErrorLabel.IsVisible = true;
                    }
                    else
                    {
                        RecaptchaErrorLabel.IsVisible = true;
                    }
                }
            }

        }

        private void PasswordTextChanged(object sender, TextChangedEventArgs e)
        {
            PassWordValidate();
        }

        private void PasswordRepeaTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != PasswordEnrty.Text)
            {
                PasswordRepeatErrorLabel.IsVisible = true;
            }
            else
            {
                PasswordRepeatErrorLabel.IsVisible = false;
            }
        }
    }
}