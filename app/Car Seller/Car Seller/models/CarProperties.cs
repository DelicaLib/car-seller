using System;
using System.Collections.Generic;
using System.Text;

namespace Car_Seller.models
{
    public abstract class CarProperties
    {
        public string City { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public MyEnums.Body Body { get; set; }
        public MyEnums.Transmission Transmission { get; set; }
        public MyEnums.Engine Engine { get; set; }
        public MyEnums.Drive Drive { get; set; }
    }
}
