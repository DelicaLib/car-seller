using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Car_Seller.views
{
    internal class MyBasePage
    {
        private ContentPage _page;
        public MyBasePage(ContentPage page) 
        {
            _page = page;
        }
        public void OnAppearing()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            if (_page.Navigation.ModalStack.Count == 0)
            {
                Connectivity_ConnectivityChanged(null, null);
            }
            CheckServerConnection();

        }

        public void OnDisappearing()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        public async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                var noInternetPage = new NoInternetPage();
                await _page.Navigation.PushModalAsync(noInternetPage);
            }
        }

        public async void CheckServerConnection()
        {
            try
            {
                await ServerInteraction.Ping();
            }
            catch (HttpRequestException ex)
            {
                await GoToNoServerConnectionPage();
            }
        }

        public async Task GoToNoServerConnectionPage()
        {
            var noServerConnectionPage = new NoServerConnectionPage();
            await _page.Navigation.PushModalAsync(noServerConnectionPage);
        }
    }
}
