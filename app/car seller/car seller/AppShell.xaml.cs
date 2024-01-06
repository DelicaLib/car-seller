using car_seller.ViewModels;
using car_seller.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace car_seller
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
