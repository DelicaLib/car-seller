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
        public async Task<bool> OnAppearing()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            if (_page.Navigation.ModalStack.Count == 0)
            {
                if (!await CheckInrenet())
                {
                    return false;
                }
            }
            return await CheckServerConnection();

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

        public async Task<bool> CheckInrenet()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                var noInternetPage = new NoInternetPage();
                await _page.Navigation.PushModalAsync(noInternetPage);
                return false;
            }
            return true;
        }

        public async Task<bool> CheckServerConnection()
        {
            try
            {
                await ServerInteraction.Ping();
                return true;
            }
            catch (HttpRequestException ex)
            {
                await GoToNoServerConnectionPage();
                return false;
            }
        }

        public async Task GoToNoServerConnectionPage()
        {
            if (_page.Navigation.ModalStack.Count == 0)
            {
                var noServerConnectionPage = new NoServerConnectionPage();
                await _page.Navigation.PushModalAsync(noServerConnectionPage);
            }
        }
    }
}
