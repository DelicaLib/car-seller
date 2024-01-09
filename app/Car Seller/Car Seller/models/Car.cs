using System;
using System.Collections.Generic;
using System.Text;

namespace Car_Seller.models
{
    internal class Car : CarProperties
    {
        public int Id { get; set; }
        public int Mileage { get; set; }
        public int ReleaseYear { get; set; }
        public long Cost { get; set; }
        public long Volume { get; set; }
        public string Description {  get; set; }
        public List<string> pPhotos { get; set; }
        public DateTime DatePublush { get; set; }
        public int OwnerId { get; set; }

    }
}
