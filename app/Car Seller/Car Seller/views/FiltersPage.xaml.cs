using Car_Seller.models;
using Car_Seller.services;
using Car_Seller.viewModels;
using RangeSelection;
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
        private string minText = string.Empty;
        private string maxText = string.Empty;
        private long minSelectValue = 0;
        private long maxSelectValue = 10000;

        public FiltersPage()
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            InitializeComponent();
            m_BasePage = new MyBasePage(this);
        }
        protected async override void OnAppearing()
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            base.OnAppearing();
            if (!await m_BasePage.OnAppearing())
            {
                return;
            }
            viewModel = new FilterPageViewModel();
            SetCurrentFilter();
            SetAvailableFiltersAsync();
            BindingContext = viewModel;
            RootLayout.WidthRequest = Application.Current.MainPage.Width;
            RootLayout.Children.Add(
                TextSelectMenu,
                Constraint.Constant(0),
                Constraint.RelativeToParent(parent => parent.Height - TextSelectMenu.HeightRequest),
                Constraint.RelativeToParent(parent => parent.Width),
                Constraint.Constant(TextSelectMenu.HeightRequest));
            await TextSelectMenu.TranslateTo(0, TextSelectMenu.HeightRequest, 250, Easing.SinInOut);
            RootLayout.Children.Add(
                NumberSelectMenu,
                Constraint.Constant(0),
                Constraint.RelativeToParent(parent => parent.Height - NumberSelectMenu.HeightRequest),
                Constraint.RelativeToParent(parent => parent.Width),
                Constraint.Constant(NumberSelectMenu.HeightRequest));
            await NumberSelectMenu.TranslateTo(0, NumberSelectMenu.HeightRequest, 250, Easing.SinInOut);
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
                DataStore.CurrentFilter.Model = null;
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
                VolumeCancelImage.IsVisible = false;
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
                CostCancelImage.IsVisible = false;
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
                MileageCancelImage.IsVisible = false;
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
                ReleaseYearCancelImage.IsVisible = false;
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
            if (NumberSelectMenu.IsVisible)
            {
                await ToggleSelectMenu(NumberSelectMenu);
            }
        }

        private void SubmitClicked(object sender, EventArgs e)
        {
            if (((List<long>)tempFilter[MinValueEntry.@class[0]])[0] < minSelectValue && ((List<long>)tempFilter[MinValueEntry.@class[0]])[0] != -1)
            {
                DisplayAlert("Ошибка", $"Значение 'От' не должно быть меньше {minSelectValue}", "Ок");
                return;
            }
            if (((List<long>)tempFilter[MinValueEntry.@class[0]])[0] > maxSelectValue && ((List<long>)tempFilter[MinValueEntry.@class[0]])[0] != -1)
            {
                DisplayAlert("Ошибка", $"Значение 'От' не должно быть больше {maxSelectValue}", "Ок");
                return;
            }
            if (((List<long>)tempFilter[MinValueEntry.@class[0]])[1] > maxSelectValue && ((List<long>)tempFilter[MinValueEntry.@class[0]])[1] != -1)
            {
                DisplayAlert("Ошибка", $"Значение 'До' не должно быть больше {maxSelectValue}", "Ок");
                return;
            }
            if (((List<long>)tempFilter[MinValueEntry.@class[0]])[1] < minSelectValue && ((List<long>)tempFilter[MinValueEntry.@class[0]])[1] != -1)
            {
                DisplayAlert("Ошибка", $"Значение 'До' не должно быть меньше {minSelectValue}", "Ок");
                return;
            }
            if ((tempFilter.MinMileage > tempFilter.MaxMileage && tempFilter.MaxMileage != -1) || (tempFilter.MinCost > tempFilter.MaxCost && tempFilter.MaxCost != -1) || (tempFilter.MinReleaseYear > tempFilter.MaxReleaseYear && tempFilter.MaxReleaseYear != -1) || (tempFilter.MinVolume > tempFilter.MaxVolume && tempFilter.MaxVolume != -1))
            {
                DisplayAlert("Ошибка", "Значение 'До' не должно быть меньше значения 'от'", "Ок");
                return;
            }
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


        private void MinTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 0)
            {
                minText = string.Empty;
                SelectRangeSlider.LowerValue = minSelectValue;
                return;
            }
            if (long.TryParse(e.NewTextValue, out long result) && result.ToString() == e.NewTextValue)
            {
                ((Entry)sender).Text = result.ToString();
                tempFilter[MinValueEntry.@class[0]] = new List<long> { result, (maxText.Length == 0 ? -1: long.Parse(maxText))};
                minText = result.ToString();
                SelectRangeSlider.LowerValue = result;
            }
            else
            {
                ((Entry)sender).Text = minText;
            }
        }
        private void MaxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 0)
            {
                maxText = string.Empty;
                SelectRangeSlider.UpperValue = maxSelectValue;
                return;
            }
            if (long.TryParse(e.NewTextValue, out long result) && result.ToString() == e.NewTextValue)
            {
                ((Entry)sender).Text = result.ToString();
                maxText = result.ToString();
                tempFilter[MinValueEntry.@class[0]] = new List<long> {(minText.Length == 0 ? -1 : long.Parse(minText)), result };
                SelectRangeSlider.UpperValue = result;
            }
            else
            {
                ((Entry)sender).Text = maxText;
            }
        }

        private void RangeSliderChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            long minTextTmp = 0;
            if (minText.Length != 0)
            {
                minTextTmp = long.Parse(minText);
            }
            long maxTextTmp = 50000000;
            if (maxText.Length != 0)
            {
                maxTextTmp = long.Parse(maxText);
            }
            if (e.PropertyName == "LowerValue" && (long)((RangeSlider)sender).LowerValue != minTextTmp)
            {
                minText = ((long)((RangeSlider)sender).LowerValue).ToString();
                MinValueEntry.Text = minText;
            }
            else if (e.PropertyName == "UpperValue" && (long)((RangeSlider)sender).UpperValue != maxTextTmp)
            {
                maxText = ((long)((RangeSlider)sender).UpperValue).ToString();
                MaxValueEntry.Text = maxText;
            }

        }

        private void SetNumberMenuValues(long minimum, long maximum, long currentMinimum, long currentMaximum)
        {
            minSelectValue = minimum;
            maxSelectValue = maximum;
            MinValueEntry.Placeholder = minimum.ToString();
            MaxValueEntry.Placeholder = maximum.ToString();
            SelectRangeSlider.MinimumValue = minimum;
            SelectRangeSlider.MaximumValue = maximum;

            if (currentMinimum == -1)
            {
                MinValueEntry.Text = string.Empty;
                minText = string.Empty;
                SelectRangeSlider.LowerValue = minimum;
            }
            else
            {
                MinValueEntry.Text = currentMinimum.ToString();
                minText = currentMinimum.ToString();
                SelectRangeSlider.LowerValue = currentMinimum;
            }

            if (currentMaximum == -1)
            {
                MaxValueEntry.Text = string.Empty;
                maxText = string.Empty;
                SelectRangeSlider.UpperValue = maximum;
            }
            else
            {
                MaxValueEntry.Text = currentMaximum.ToString();
                maxText = currentMaximum.ToString();
                SelectRangeSlider.UpperValue = currentMaximum;
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

        private async void OnCostClicked(object sender, EventArgs e)
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            NumberSelectMenuHeader.Text = "Цена, ₽";
            MinValueEntry.@class = new List<string>() { "цена" };
            MaxValueEntry.@class = new List<string>() { "цена" };
            SetNumberMenuValues(0, 50000000, tempFilter.MinCost, tempFilter.MaxCost);
            await ToggleSelectMenu(NumberSelectMenu);
        }

        private async void OnMileageClicked(object sender, EventArgs e)
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            NumberSelectMenuHeader.Text = "Пробег, км";
            MinValueEntry.@class = new List<string>() { "пробег" };
            MaxValueEntry.@class = new List<string>() { "пробег" };
            SetNumberMenuValues(0, 500000, tempFilter.MinMileage, tempFilter.MaxMileage);
            await ToggleSelectMenu(NumberSelectMenu);
        }

        private async void OnVolumeClicked(object sender, EventArgs e)
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            NumberSelectMenuHeader.Text = "Мощность двигателя, л.с";
            MinValueEntry.@class = new List<string>() { "мощность" };
            MaxValueEntry.@class = new List<string>() { "мощность" };
            SetNumberMenuValues(10, 1000, tempFilter.MinVolume, tempFilter.MaxVolume);
            await ToggleSelectMenu(NumberSelectMenu);
        }

        private async void OnResealeYearClicked(object sender, EventArgs e)
        {
            tempFilter = (Filter)DataStore.CurrentFilter.Clone();
            NumberSelectMenuHeader.Text = "Год выпуска";
            MinValueEntry.@class = new List<string>() { "год" };
            MaxValueEntry.@class = new List<string>() { "год" };
            SetNumberMenuValues(0, 2024, tempFilter.MinReleaseYear, tempFilter.MaxReleaseYear);
            await ToggleSelectMenu(NumberSelectMenu);
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
            TextSelectMenuRadioNone.IsChecked = DataStore.CurrentFilter.Model == null;
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