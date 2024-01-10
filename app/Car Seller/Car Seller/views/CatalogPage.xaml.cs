using Car_Seller.models;
using Car_Seller.services;
using Car_Seller.viewModels;
using Car_Seller.views;
using IntelliAbb.Xamarin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
        private Filter oldFilter;
        public CatalogPage()
        {
            viewModel = new CatalogPageViewModel(dataStore, m_BasePage);
            m_BasePage = new MyBasePage(this);
            oldFilter = (Filter)dataStore.CurrentFilter.Clone();
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            if (!await m_BasePage.OnAppearing())
            {
                return;
            }
            if (!oldFilter.Equals(dataStore.CurrentFilter))
            {
                viewModel.Page = 1;
                viewModel.PageSize = 10;
                oldFilter = (Filter)dataStore.CurrentFilter.Clone();
            }
            await viewModel.GenerateCars();
            base.OnAppearing();
            RootCollectionView.ItemsSource = viewModel.GetCopy();
            PageLabel.Text = $"Страница: {viewModel.Page}";

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
            Label carIdlabel = (Label)((StackLayout)((TemplatedView)sender).Parent).Children[0];
            if (carIdlabel.Text == null)
            {
                return;
            }
            int carId = int.Parse(carIdlabel.Text);
            Car.CarForView currentCar = null;
            foreach (var item in viewModel.cars)
            {
                if (item.car.Id == carId)
                {
                    currentCar = item;
                    if (((Checkbox)sender).IsChecked == item.IsLiked)
                    {
                        return;
                    }
                    break;
                }
            }
            if (!(await ServerInteraction.HasJWTAsync()))
            {
                ((Checkbox)sender).IsChecked = false;
                await Shell.Current.GoToAsync("LoginPage");
                return;
            }
            if (((Checkbox)sender).IsChecked)
            {
                try
                {
                    await ServerInteraction.SetLike(carId);
                    currentCar.IsLiked = true;
                }
                catch (HttpRequestException ex)
                {
                    ((Checkbox)sender).IsChecked = false;
                    await m_BasePage.GoToNoServerConnectionPage();
                    return;
                }
                catch (WebException ex)
                {
                    if (ex.Status == (WebExceptionStatus)401)
                    {
                        ((Checkbox)sender).IsChecked = false;
                        await Shell.Current.GoToAsync("LoginPage");
                        return;
                    }
                    else
                    {

                        return;
                    }
                }
            }
            else
            {
                try
                {
                    await ServerInteraction.DeleteLike(carId);
                    currentCar.IsLiked = false;
                }
                catch (HttpRequestException ex)
                {
                    ((Checkbox)sender).IsChecked = true;
                    await m_BasePage.GoToNoServerConnectionPage();
                    return;
                }
                catch (WebException ex)
                {
                    ((Checkbox)sender).IsChecked = true;
                    if (ex.Status == (WebExceptionStatus)401)
                    {
                        await Shell.Current.GoToAsync("LoginPage");
                        return;
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", ex.Message, "Ок(");
                        return;
                    }
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

        private async void NextClicked(object sender, EventArgs e)
        {
            viewModel.Page++;
            if (!await viewModel.GenerateCars())
            {
                return;
            }
            RootCollectionView.ItemsSource = viewModel.cars;
            PageLabel.Text = $"Страница: {viewModel.Page}";
        }
        private async void PreviousClicked(object sender, EventArgs e)
        {
            if (viewModel.Page == 1)
            {
                return;
            }
            viewModel.Page--;
            if (!await viewModel.GenerateCars())
            {
                return;
            }
            RootCollectionView.ItemsSource = viewModel.cars;
            PageLabel.Text = $"Страница: {viewModel.Page}";
        }
        private async void GoToFirstClicked(object sender, EventArgs e)
        {
            if (viewModel.Page == 1)
            {
                return;
            }
            viewModel.Page = 1;
            if (!await viewModel.GenerateCars())
            {
                return;
            }
            RootCollectionView.ItemsSource = viewModel.cars;
            PageLabel.Text = $"Страница: {viewModel.Page}";
        }
    }
}
