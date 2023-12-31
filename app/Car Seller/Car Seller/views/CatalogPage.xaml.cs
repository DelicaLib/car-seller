﻿using Car_Seller.views;
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
        public CatalogPage()
        {
            InitializeComponent();
            m_BasePage = new MyBasePage(this);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            m_BasePage.OnAppearing();

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
    }
}
