using Car_Seller.models;
using Car_Seller.services;
using Car_Seller.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Car_Seller.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FiltersPage : ContentPage
    {
        private Filter tempFilter;
        private DataStore DataStore = DependencyService.Get<DataStore>();
        private MyBasePage m_BasePage;
        private FilterPageViewModel viewModel;

        public FiltersPage()
        {
            InitializeComponent();
            m_BasePage = new MyBasePage(this);
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            m_BasePage.OnAppearing();
            viewModel = new FilterPageViewModel();
            SetCurrentFilter();
            SetAvailableFiltersAsync();
            BindingContext = viewModel;
            RootLayout.Children.Add(
                TextSelectMenu,
                Constraint.Constant(0),
                Constraint.RelativeToParent(parent => parent.Height - TextSelectMenu.HeightRequest),
                Constraint.RelativeToParent(parent => parent.Width),
                Constraint.Constant(TextSelectMenu.HeightRequest));
            await TextSelectMenu.TranslateTo(0, TextSelectMenu.HeightRequest, 250, Easing.SinInOut);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            m_BasePage.OnDisappearing();
        }

        public async void SetAvailableFiltersAsync()
        {
            try
            {
                viewModel.availableFilters = new AvailableFiltersForView(await ServerInteraction.GetAvailableFilters(DataStore.CurrentFilter), DataStore.CurrentFilter);
            }
            catch (HttpRequestException ex)
            {
                await m_BasePage.GoToNoServerConnectionPage();
            }
        }

        private void SetCurrentFilter()
        {
            if (DataStore.CurrentFilter.City == null)
            {
                CityLabel.Text = "Не выбран";
                CityCancelImage.IsVisible = false;
            }
            else
            {
                CityLabel.Text = DataStore.CurrentFilter.City;
                CityCancelImage.IsVisible = true;
            }
            if (DataStore.CurrentFilter.Brand == null)
            {
                BrandLabel.Text = "Не выбрана";
                BrandCancelImage.IsVisible = false;
                ModelLabel.Text = "Недоступно";
                ModelLabel.TextColor = Color.Gray;
                ModelCancelImage.IsVisible = false;
            }
            else
            {
                BrandLabel.Text = DataStore.CurrentFilter.Brand;
                BrandCancelImage.IsVisible = true;
                ModelLabel.TextColor = (Color)Application.Current.Resources["SecondaryColor"];
                if (DataStore.CurrentFilter.Model == null)
                {
                    ModelLabel.Text = "Не выбрана";
                    ModelCancelImage.IsVisible = false;
                }
                else
                {
                    ModelLabel.Text = DataStore.CurrentFilter.Model;
                    ModelCancelImage.IsVisible = true;
                }
            }

            if (DataStore.CurrentFilter.Body == null)
            {
                BodyLabel.Text = "Не выбран";
                BodyCancelImage.IsVisible = false;
            }
            else
            {
                BodyLabel.Text = DataStore.CurrentFilter.Body.ToString();
                BodyCancelImage.IsVisible = true;
            }

            if (DataStore.CurrentFilter.Transmission == null)
            {
                TransmissionLabel.Text = "Не выбрана";
                TransmissionCancelImage.IsVisible = false;
            }
            else
            {
                TransmissionLabel.Text = DataStore.CurrentFilter.Transmission.ToString();
                TransmissionCancelImage.IsVisible = true;
            }

            if (DataStore.CurrentFilter.Engine == null)
            {
                EngineLabel.Text = "Не выбран";
                EngineCancelImage.IsVisible = false;
            }
            else
            {
                EngineLabel.Text = DataStore.CurrentFilter.Engine.ToString();
                EngineCancelImage.IsVisible = true;
            }

            if (DataStore.CurrentFilter.Drive == null)
            {
                DriveLabel.Text = "Не выбран";
                DriveCancelImage.IsVisible = false;
            }
            else
            {
                DriveLabel.Text = DataStore.CurrentFilter.Drive.ToString();
                DriveCancelImage.IsVisible = true;
            }

            if (DataStore.CurrentFilter.MinVolume == -1 && DataStore.CurrentFilter.MaxVolume == -1)
            {
                VolumeLabel.Text = "Не выбрана";
            }
            else
            {
                VolumeLabel.Text = "";
                VolumeCancelImage.IsVisible = true;
                if (DataStore.CurrentFilter.MinVolume != -1)
                {
                    VolumeLabel.Text = $"от {DataStore.CurrentFilter.MinVolume} ";
                }
                if (DataStore.CurrentFilter.MaxVolume != -1)
                {
                    VolumeLabel.Text = $"{VolumeLabel.Text}до {DataStore.CurrentFilter.MaxVolume}";
                }
            }

            if (DataStore.CurrentFilter.MinCost == -1 && DataStore.CurrentFilter.MaxCost == -1)
            {
                CostLabel.Text = "Не выбрана";
            }
            else
            {
                CostLabel.Text = "";
                CostCancelImage.IsVisible = true;
                if (DataStore.CurrentFilter.MinCost != -1)
                {
                    CostLabel.Text = $"от {DataStore.CurrentFilter.MinCost} ";
                }
                if (DataStore.CurrentFilter.MaxCost != -1)
                {
                    CostLabel.Text = $"{CostLabel.Text}до {DataStore.CurrentFilter.MaxCost}";
                }
            }

            if (DataStore.CurrentFilter.MinMileage == -1 && DataStore.CurrentFilter.MaxMileage == -1)
            {
                MileageLabel.Text = "Не выбран";
            }
            else
            {
                MileageLabel.Text = "";
                MileageCancelImage.IsVisible = true;
                if (DataStore.CurrentFilter.MinMileage != -1)
                {
                    MileageLabel.Text = $"от {DataStore.CurrentFilter.MinMileage} ";
                }
                if (DataStore.CurrentFilter.MaxMileage != -1)
                {
                    MileageLabel.Text = $"{MileageLabel.Text}до {DataStore.CurrentFilter.MaxMileage}";
                }
            }

            if (DataStore.CurrentFilter.MinReleaseYear == -1 && DataStore.CurrentFilter.MaxReleaseYear == -1)
            {
                ReleaseYearLabel.Text = "Не выбран";
            }
            else
            {
                ReleaseYearLabel.Text = "";
                ReleaseYearCancelImage.IsVisible = true;
                if (DataStore.CurrentFilter.MinReleaseYear != -1)
                {
                    ReleaseYearLabel.Text = $"от {DataStore.CurrentFilter.MinReleaseYear} ";
                }
                if (DataStore.CurrentFilter.MaxReleaseYear != -1)
                {
                    ReleaseYearLabel.Text = $"{ReleaseYearLabel.Text}до {DataStore.CurrentFilter.MaxReleaseYear}";
                }
            }
        }


        private async Task ToggleSelectMenu(VisualElement menu)
        {
            if (menu.IsVisible)
            {
                MenuBackgroundLayout.IsVisible = false;
                await menu.TranslateTo(0, menu.HeightRequest, 250, Easing.SinInOut);
                menu.IsVisible = false;
            }
            else
            {
                MenuBackgroundLayout.IsVisible = true;
                menu.IsVisible = true;
                await menu.TranslateTo(0, 0, 250, Easing.SinInOut);
            }
        }

        private async void CloseAllMenu(object sender, EventArgs e)
        {
            if (TextSelectMenu.IsVisible)
            {
                await ToggleSelectMenu(TextSelectMenu);
            }
        }

        private void SubmitClicked(object sender, EventArgs e)
        {
            DataStore.CurrentFilter = tempFilter;
            SetAvailableFiltersAsync();
            SetCurrentFilter();
            CloseAllMenu(sender, e);
        }

        private void RadioCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value && ((RadioButton)sender).Value != null)
            {
                string filterName = TextSelectMenuHeader.Text;
                if (((RadioButton)sender).Value.ToString() == "-1")
                {
                    tempFilter.DeleteElement(filterName);
                }
                else
                {

                    tempFilter[filterName] = ((RadioButton)sender).Value.ToString();
                }
            }
        }


        private async void OnCityClicked(object sender, EventArgs e)
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            TextSelectMenuHeader.Text = "Город";
            TextSelectMenuCollectionView.ItemsSource = viewModel.availableFilters.City;
            TextSelectMenuRadioNone.IsChecked = DataStore.CurrentFilter.City == null;
            await ToggleSelectMenu(TextSelectMenu);
        }
        private async void OnBrandClicked(object sender, EventArgs e)
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            TextSelectMenuHeader.Text = "Марка";
            TextSelectMenuCollectionView.ItemsSource = viewModel.availableFilters.Brand;
            TextSelectMenuRadioNone.IsChecked = DataStore.CurrentFilter.Brand == null;
            await ToggleSelectMenu(TextSelectMenu);
        }
        private async void OnModelClicked(object sender, EventArgs e)
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            if (DataStore.CurrentFilter.Brand == null)
            {
                return;
            }
            TextSelectMenuHeader.Text = "Модель";
            TextSelectMenuCollectionView.ItemsSource = viewModel.availableFilters.Model;
            await ToggleSelectMenu(TextSelectMenu);
        }
        private async void OnBodyClicked(object sender, EventArgs e)
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            TextSelectMenuHeader.Text = "Кузов";
            TextSelectMenuCollectionView.ItemsSource = viewModel.availableFilters.Body;
            TextSelectMenuRadioNone.IsChecked = DataStore.CurrentFilter.Body == null;
            await ToggleSelectMenu(TextSelectMenu);
        }
        private async void OnTransmissionClicked(object sender, EventArgs e)
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            TextSelectMenuHeader.Text = "Коробка передач";
            TextSelectMenuCollectionView.ItemsSource = viewModel.availableFilters.Transmission;
            TextSelectMenuRadioNone.IsChecked = DataStore.CurrentFilter.Transmission is null;
            await ToggleSelectMenu(TextSelectMenu);
        }
        private async void OnDriveClicked(object sender, EventArgs e)
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            TextSelectMenuHeader.Text = "Привод";
            TextSelectMenuCollectionView.ItemsSource = viewModel.availableFilters.Drive;
            TextSelectMenuRadioNone.IsChecked = DataStore.CurrentFilter.Drive == null;
            await ToggleSelectMenu(TextSelectMenu);
        }
        private async void OnEngineClicked(object sender, EventArgs e)
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            TextSelectMenuHeader.Text = "Тип двигателя";
            TextSelectMenuCollectionView.ItemsSource = viewModel.availableFilters.Engine;
            TextSelectMenuRadioNone.IsChecked = DataStore.CurrentFilter.Engine == null;
            await ToggleSelectMenu(TextSelectMenu);
        }


        private void OnCityRemove(object sender, EventArgs e)
        {
            DataStore.CurrentFilter.City = null;
            SetCurrentFilter();
            SetAvailableFiltersAsync();
        }
        private void OnBrandRemove(object sender, EventArgs e)
        {
            DataStore.CurrentFilter.Brand = null;
            SetCurrentFilter();
            SetAvailableFiltersAsync();
        }
        private void OnModelRemove(object sender, EventArgs e)
        {
            DataStore.CurrentFilter.Model = null;
            SetCurrentFilter();
            SetAvailableFiltersAsync();
        }
        private void OnBodyRemove(object sender, EventArgs e)
        {
            DataStore.CurrentFilter.Body = null;
            SetCurrentFilter();
            SetAvailableFiltersAsync();
        }
        private void OnTransmissionRemove(object sender, EventArgs e)
        {
            DataStore.CurrentFilter.Transmission = null;
            SetCurrentFilter();
            SetAvailableFiltersAsync();
        }
        private void OnEngineRemove(object sender, EventArgs e)
        {
            DataStore.CurrentFilter.Engine = null;
            SetCurrentFilter();
            SetAvailableFiltersAsync();
        }
        private void OnDriveRemove(object sender, EventArgs e)
        {
            DataStore.CurrentFilter.Drive = null;
            SetCurrentFilter();
            SetAvailableFiltersAsync();
        }
        private void OnVolumeRemove(object sender, EventArgs e)
        {
            DataStore.CurrentFilter.MinVolume = -1;
            DataStore.CurrentFilter.MaxVolume = -1;
            SetCurrentFilter();
            SetAvailableFiltersAsync();
        }
        private void OnCostRemove(object sender, EventArgs e)
        {
            DataStore.CurrentFilter.MinCost = -1;
            DataStore.CurrentFilter.MaxCost = -1;
            SetCurrentFilter();
            SetAvailableFiltersAsync();
        }
        private void OnMileageRemove(object sender, EventArgs e)
        {
            DataStore.CurrentFilter.MinMileage = -1;
            DataStore.CurrentFilter.MaxMileage = -1;
            SetCurrentFilter();
            SetAvailableFiltersAsync();
        }
        private void OnReleaseYearRemove(object sender, EventArgs e)
        {
            DataStore.CurrentFilter.MinReleaseYear = -1;
            DataStore.CurrentFilter.MaxReleaseYear = -1;
            SetCurrentFilter();
            SetAvailableFiltersAsync();
        }
    }
}