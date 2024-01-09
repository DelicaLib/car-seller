using Car_Seller.models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Car_Seller.services
{
    internal class DataStore
    {
        private Filter currentFilter;
        private int currentFilterPage = 1;
        private int filterPageSize = 10;
        private int currentLikePage = 1;
        private int likePageSize = 10;
        private int currentResponsePage = 1;
        private int responsePageSize = 10;

        public Filter CurrentFilter
        {
            get
            { return currentFilter; }
            set { currentFilter = value; }
        }

        public DataStore() 
        {
            currentFilter = new Filter()
            {
                Body = null,
                Brand = null,
                City = null,
                Drive = null,
                Engine = null,
                MaxCost = -1,
                MaxMileage = -1,
                MaxReleaseYear = -1,
                MaxVolume = -1,
                MinCost = -1,
                MinMileage = -1,
                MinReleaseYear = -1,
                MinVolume = -1,
                Model = null,
                Transmission = null
            };
        }
    }
}
