using car_seller.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace car_seller.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}