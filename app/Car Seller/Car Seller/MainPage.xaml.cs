using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Car_Seller
{
    public partial class MainPage : ContentPage
    {
        private string HOST = "192.168.0.131";
        private string PORT = "8000";
        public MainPage()
        {
            InitializeComponent();

            recaptchaView.Source = "http://" + HOST + ":" + PORT + "/recaptcha_form";
        }
    }
}
