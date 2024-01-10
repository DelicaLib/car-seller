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

        public async Task<bool> FindCars()
        {
            try
            {
                foundCars = await ServerInteraction.GetCarsAsync(dataStore.CurrentFilter, PageSize, Page);
                return true;
            }
            catch (HttpRequestException ex)
            {
                await m_basePage.GoToNoServerConnectionPage();
                return false;
            }
        }

        public async Task<bool> GenerateCars()
        {
            if (!await FindCars())
            {
                return false;
            }
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
                    return false;
                }
                cars.Add(new Car.CarForView(car, isLiked));
            }
            return true;
        }

        public List<Car.CarForView> GetCopy()
        {
            List<Car.CarForView> result = new List<Car.CarForView>();
            foreach (var car in cars)
            {
                result.Add(new Car.CarForView(car.car, car.IsLiked));
            }
            return result;
        }
    }
}
