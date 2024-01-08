using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Car_Seller.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FiltersPage : ContentPage
    {
        private MyBasePage m_BasePage;
        public FiltersPage()
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
    }
}