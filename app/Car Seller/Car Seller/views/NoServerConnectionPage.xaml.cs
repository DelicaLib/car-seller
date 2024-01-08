using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Car_Seller.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoServerConnectionPage : ContentPage
    {
        public NoServerConnectionPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private async void onRetryButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string oldText = button.Text;
            button.Text = "Подождите...";
            button.Clicked -= onRetryButtonClicked;
            CheckServerConnection();
            await Task.Delay(3000);
            button.Clicked += onRetryButtonClicked;
            button.Text = oldText;
        }

        public async void CheckServerConnection()
        {
            try
            {
                await ServerInteraction.Ping();
                await Navigation.PopModalAsync(true);
            }
            catch (HttpRequestException ex)
            {
                
            }
        }

    }
}