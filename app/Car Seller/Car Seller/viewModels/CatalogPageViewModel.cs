using Car_Seller.models;
using Car_Seller.services;
using Car_Seller.views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Car_Seller.viewModels
{
    internal class CatalogPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public List<Car.CarForView> cars { get; set; }
        public int PageSize = 10;
        public int Page = 1;
        private List<Car> foundCars;
        public DataStore dataStore;
        public MyBasePage m_basePage;

        public CatalogPageViewModel(DataStore dataStore, MyBasePage myBasePage)
        {
            m_basePage = myBasePage;
            this.dataStore = dataStore;

        }

        public async Task FindCars()
        {
            try
            {
                foundCars = await ServerInteraction.GetCarsAsync(dataStore.CurrentFilter, PageSize, Page);
            }
            catch (HttpRequestException ex)
            {
                await m_basePage.GoToNoServerConnectionPage();
            }
        }

        public async Task GenerateCars()
        {
            await FindCars();
            cars = new List<Car.CarForView>();
            foreach (var car in foundCars)
            {
                bool isLiked = false;
                try
                {
                    isLiked = await ServerInteraction.CarIsLiked(car.Id);
                }
                catch (HttpRequestException ex)
                {
                    await m_basePage.GoToNoServerConnectionPage();
                }
                cars.Add(new Car.CarForView(car, isLiked));
            }
        }

    }
}
