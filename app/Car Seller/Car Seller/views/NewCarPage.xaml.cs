using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Car_Seller.models.MyEnums;
using Car_Seller.models;
using Xamarin.Essentials;
using System.IO;
using DriveType = Car_Seller.models.MyEnums.DriveType;

namespace Car_Seller.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewCarPage : ContentPage
    {
        private MyBasePage m_BasePage;
        private List<ImageSource> photoPathes;
        public NewCarPage()
        {
            photoPathes = new List<ImageSource>();
            m_BasePage = new MyBasePage(this);
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            PhotosCollectionView.ItemsSource = photoPathes;
            if (!await m_BasePage.OnAppearing())
            {
                return;
            }
            RecaptchaWebView.Source = ServerInteraction.GetURL() + "recaptcha_form";
            if (!await ServerInteraction.HasJWTAsync())
            {
                await Shell.Current.GoToAsync("LoginPage");
                return;
            }
            try
            {
                await ServerInteraction.GetProfileData();
            }
            catch (HttpRequestException ex)
            {
                await m_BasePage.GoToNoServerConnectionPage();
                return;
            }
            catch (WebException ex)
            {
                await Shell.Current.GoToAsync("LoginPage");
                return;
            }
            SetPickers();
            base.OnAppearing();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            m_BasePage.OnDisappearing();
        }

        private void SetPickers()
        {
            foreach (var item in Enum.GetValues(typeof(BodyType)))
            {
                BodyPicker.Items.Add(MyEnums.ToString((BodyType)item));
            }
            foreach (var item in Enum.GetValues(typeof(TransmissionType)))
            {
                TransmissionPicker.Items.Add(MyEnums.ToString((TransmissionType)item));
            }
            foreach (var item in Enum.GetValues(typeof(EngineType)))
            {
                EnginePicker.Items.Add(MyEnums.ToString((EngineType)item));
            }
            foreach (var item in Enum.GetValues(typeof(DriveType)))
            {
                DrivePicker.Items.Add(MyEnums.ToString((DriveType)item));
            }
        }

        async Task TakePhotoAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageRead>();
                    if (status != PermissionStatus.Granted)
                    {
                        // Разрешение не предоставлено, обработайте ситуацию
                        ;
                    }
                }
                var photo = await MediaPicker.PickPhotoAsync();
                await LoadPhotoAsync(photo);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
            {
                return;
            }
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);
            photoPathes.Add(ImageSource.FromFile(newFile));
            PhotosCollectionView.ItemsSource = photoPathes;
            Huy.Source = ImageSource.FromFile(newFile);
        }

        private async void CreateClicked(object sender, EventArgs e)
        {
            await TakePhotoAsync();
        }

    }
}