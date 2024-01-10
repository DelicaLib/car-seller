using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Car_Seller.models
{
    internal class Car : CarProperties
    {
        public int Id { get; set; }
        public long Mileage { get; set; }
        public long ReleaseYear { get; set; }
        public string Cost { get; set; }
        public long Volume { get; set; }
        public string Description {  get; set; }
        public List<string> Photos { get; set; }
        public DateTime DatePublush { get; set; }
        public int OwnerId { get; set; }

        public Car() { }
        public Car(Dictionary<string, object> properties)
        {
            object tmp = properties["id"];
            Id = int.Parse(properties["id"].ToString());
            Mileage = long.Parse(properties["mileage"].ToString());
            ReleaseYear = long.Parse(properties["release_year"].ToString());
            Cost = properties["cost"].ToString().Split('.')[0];
            StringBuilder str = new StringBuilder();
            for (int i = Cost.Length - 1; i >= 0; i--)
            {
                if (Cost.Length - 1 - i != 0 && (Cost.Length - 1 - i) % 3 == 0)
                {
                    str.Append(" ");
                }
                str.Append(Cost[i]);
            }
            Cost = new string(str.ToString().ToCharArray().Reverse().ToArray());
            Volume = long.Parse(properties["volume"].ToString());
            Description = properties["description"].ToString();
            City = properties["city"].ToString();
            Brand = properties["brand"].ToString();
            Model = properties["model"].ToString();
            Body = properties["body"].ToString();
            Transmission = properties["transmission"].ToString();
            Engine = properties["engine"].ToString();
            Drive = properties["drive"].ToString();
            DatePublush = DateTime.Parse(properties["date_publish"].ToString());
            OwnerId = int.Parse(properties["id_owner"].ToString());
            Photos = new List<string> { };
            foreach (var i in ((Newtonsoft.Json.Linq.JArray)properties["photos"]).ToList())
            {
                var item = i.ToString();
                List<string> items = item.Split('/').ToList();
                Photos.Add($"{ServerInteraction.GetURL()}{items[2]}/{items[3]}/{items[4]}");
            }

        }

        public class CarForView
        {
            public Car car { get; set; } 
            public bool IsLiked { get; set; }

            public CarForView(Car car, bool isLiked) 
            { 
                this.car = car;
                this.IsLiked = isLiked;
            }

        }
            
        
    }
}
