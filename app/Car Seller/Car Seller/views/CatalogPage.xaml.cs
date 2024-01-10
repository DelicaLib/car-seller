using Car_Seller.services;
using Car_Seller.viewModels;
using Car_Seller.views;
using IntelliAbb.Xamarin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Car_Seller.views
{
    public partial class CatalogPage : ContentPage
    {
        private MyBasePage m_BasePage;
        private DataStore dataStore = DependencyService.Get<DataStore>();
        private CatalogPageViewModel viewModel;
        public CatalogPage()
        {
            viewModel = new CatalogPageViewModel(dataStore, m_BasePage);
            m_BasePage = new MyBasePage(this);
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            if (!await m_BasePage.OnAppearing())
            {
                return;
            }
            await viewModel.GenerateCars();
            base.OnAppearing();
            BindingContext = viewModel;

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            m_BasePage.OnDisappearing();
        }

        private async void OnFiltersClicked(object sender, EventArgs e)
        {
            var filtersPage = new FiltersPage();
            await Navigation.PushAsync(filtersPage, true);
        }

        private async void IsLikedChanged(object sender, TappedEventArgs e)
        {
            if ((bool)e.Parameter)
            {
                int carId = int.Parse(((Checkbox)sender).ClassId);
                if (!(await ServerInteraction.HasJWTAsync()))
                {
                    ((Checkbox)sender).IsChecked = false;
                    await Shell.Current.GoToAsync("LoginPage");
                    return;
                }
            }
        }

        private async void ResponseClicked(object sender, EventArgs e)
        {
            int carId = int.Parse(((Button)sender).ClassId);
            if (!(await ServerInteraction.HasJWTAsync()))
            {
                await Shell.Current.GoToAsync("LoginPage");
                return;
            }
        }

        private async void OnCarClicked(object sender, EventArgs e)
        {
            int carId = int.Parse(((RelativeLayout)sender).ClassId);
            await Shell.Current.GoToAsync("CarPage");
        }
    }
}
