﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Car_Seller.models
{
    
    public class Filter : CarProperties , ICloneable
    {
        public long MinVolume { get; set; }
        public long MaxVolume { get; set; }
        public long MinCost { get; set;}
        public long MaxCost { get; set;}
        public long MinMileage { get; set; }
        public long MaxMileage { get; set; }
        public long MinReleaseYear { get; set; }
        public long MaxReleaseYear { get; set; }


        private object GetValue(string s)
        {
            s = s.ToLower();
            if (s == "город")
            {
                return City;
            }
            if (s == "марка")
            {
                return Brand;
            }
            if (s == "модель")
            {
                return Model;
            }
            if (s == "кузов")
            {
                return Body;
            }
            if (s == "коробка передач")
            {
                return Transmission;
            }
            if (s == "тип двигателя")
            {
                return Engine;
            }
            if (s == "привод")
            {
                return Drive;
            }
            if (s == "мощность")
            {
                return new List<long>() { MinVolume, MaxVolume };
            }
            if (s == "цена")
            {
                return new List<long>() { MinCost, MaxCost };
            }
            if (s == "пробег")
            {
                return new List<long>() { MinMileage, MaxMileage };
            }
            if (s == "год")
            {
                return new List<long>() { MinReleaseYear, MaxReleaseYear };
            }
            throw new ArgumentException("Неизвестное значение: " + s);
        }

        public void DeleteElement(string s)
        {
            s = s.ToLower();
            if (s == "город")
            {
                City = null;
            }
            if (s == "марка")
            {
                Brand = null;
            }
            if (s == "модель")
            {
                Model = null;
            }
            if (s == "кузов")
            {
                Body = null;
            }
            if (s == "коробка передач")
            {
                Transmission = null;
            }
            if (s == "тип двигателя")
            {
                Engine = null;
            }
            if (s == "привод")
            {
                Drive = null;
            }
        }

        private void SetValue(string s, object value)
        {
            s = s.ToLower();
            if (s == "город")
            {
                City = (string)value;
            }
            if (s == "марка")
            {
                Brand = (string)value;
            }
            if (s == "модель")
            {
                Model = (string)value;
            }
            if (s == "кузов")
            {
                Body = ((string)value).ToLower();
            }
            if (s == "коробка передач")
            {
                Transmission = ((string)value).ToLower();
            }
            if (s == "тип двигателя")
            {
                Engine = ((string)value).ToLower();
            }
            if (s == "привод")
            {
                Drive = ((string)value).ToLower();
            }
            if (s == "мощность")
            {
                MinVolume = ((List<long>)value)[0];
                MaxVolume = ((List<long>)value)[1];
            }
            if (s == "цена")
            {
                MinCost = ((List<long>)value)[0];
                MaxCost = ((List<long>)value)[1];
            }
            if (s == "пробег")
            {
                MinMileage = ((List<long>)value)[0];
                MaxMileage = ((List<long>)value)[1];
            }
            if (s == "год")
            {
                MinReleaseYear = ((List<long>)value)[0];
                MaxReleaseYear = ((List<long>)value)[1];
            }
        }
        public object this[string s]
        {
            get { return GetValue(s); }
            set { SetValue(s, value); }
        }

        public string GetUrlParams()
        {
            List<string> parametrs = new List<string>();
            if (City != null)
            {
                parametrs.Add($"city={City}");
            }
            if (Brand != null)
            {
                parametrs.Add($"brand={Brand}");
            }
            if (Model != null)
            {
                parametrs.Add($"model={Model}");
            }
            if (Body != null)
            {
                parametrs.Add($"body={Body}");
            }
            if (Transmission != null)
            {
                parametrs.Add($"transmission={Transmission}");
            }
            if (Engine != null)
            {
                parametrs.Add($"engine={Engine}");
            }
            if (Drive != null)
            {
                parametrs.Add($"drive={Drive}");
            }
            if (MinVolume != -1)
            {
                parametrs.Add($"min_volume={MinVolume}");
            }
            if (MaxVolume != -1)
            {
                parametrs.Add($"max_volume={MaxVolume}");
            }
            if (MinCost != -1)
            {
                parametrs.Add($"min_cost={MinCost}");
            }
            if (MaxCost != -1)
            {
                parametrs.Add($"max_cost={MaxCost}");
            }
            if (MinMileage != -1)
            {
                parametrs.Add($"min_mileage={MinMileage}");
            }
            if (MaxMileage != -1)
            {
                parametrs.Add($"max_mileage={MaxMileage}");
            }
            if (MinReleaseYear != -1)
            {
                parametrs.Add($"min_release_year={MinReleaseYear}");
            }
            if (MaxReleaseYear != -1)
            {
                parametrs.Add($"max_release_year={MaxReleaseYear}");
            }
            return string.Join("&", parametrs.ToArray());
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Filter test = obj as Filter;
            return 
                City == test.City && 
                Brand == test.Brand && 
                Model == test.Model && 
                Body == test.Body && 
                Transmission == test.Transmission && 
                Engine == test.Engine && 
                Drive == test.Drive &&
                MinMileage == test.MinMileage && 
                MaxMileage == test.MaxMileage && 
                MinCost == test.MinCost && 
                MaxCost == test.MaxCost && 
                MinReleaseYear == test.MinReleaseYear &&
                MaxReleaseYear == test.MaxReleaseYear && 
                MinVolume == test.MinVolume && 
                MaxVolume == test.MaxVolume;
        }
    }
}
