using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace Car_Seller.models
{
    public class AvailableFilters
    {
        public List<string> City { get; set; }
        public List<string> Brand { get; set; }
        public List<string> Model { get; set; }
        public List<MyEnums.Body> Body { get; set; }
        public List<MyEnums.Transmission> Transmission { get; set; }
        public List<MyEnums.Engine> Engine { get; set; }
        public List<MyEnums.Drive> Drive { get; set; }
    }

    public class AvailableFiltersForView
    {
        public class Field
        {
            public string Name { get; set; }
            public string IsChecked { get; set; }
        }
        public List<Field> City { get; set; }
        public List<Field> Brand { get; set; }
        public List<Field> Model { get; set; }
        public List<Field> Body { get; set; }
        public List<Field> Transmission { get; set; }
        public List<Field> Engine { get; set; }
        public List<Field> Drive { get; set; }

        public AvailableFiltersForView(AvailableFilters availableFilters, Filter filter)
        {
            City = new List<Field>();
            foreach (var field in availableFilters.City)
            {
                bool IsChecked = false;
                if (filter.City != null)
                {
                    IsChecked = filter.City.ToString() == field.ToString();
                }
                City.Add(new Field()
                {
                    Name = field.Substring(0, 1).ToUpper() + field.Substring(1),
                    IsChecked = IsChecked.ToString()
                });
            }
            Brand = new List<Field>();
            foreach (var field in availableFilters.Brand)
            {
                bool IsChecked = false;
                if (filter.Brand != null)
                {
                    IsChecked = filter.Brand.ToString() == field.ToString();
                }
                Brand.Add(new Field()
                {
                    Name = field.Substring(0, 1).ToUpper() + field.Substring(1),
                    IsChecked = IsChecked.ToString()
                });
            }
            if (availableFilters.Model != null)
            {
                Model = new List<Field>();
                foreach (var field in availableFilters.Model)
                {
                    bool IsChecked = false;
                    if (filter.Model != null)
                    {
                        IsChecked = filter.Model.ToString() == field.ToString();
                    }
                    Model.Add(new Field()
                    {
                        Name = field.Substring(0, 1).ToUpper() + field.Substring(1),
                        IsChecked = IsChecked.ToString()
                    });
                }
            }
            Body = new List<Field>();
            foreach (var field in availableFilters.Body)
            {
                bool IsChecked = false;
                if (filter.Body != null)
                {
                    IsChecked = filter.Body.ToString() == field.ToString();
                }

                Body.Add(new Field()
                {
                    Name = field.ToString().Substring(0, 1).ToUpper() + field.ToString().Substring(1),
                    IsChecked = IsChecked.ToString()
                });
            }
            Transmission = new List<Field>();
            foreach (var field in availableFilters.Transmission)
            {
                bool IsChecked = false;
                if (filter.Transmission != null)
                {
                    IsChecked = filter.Transmission.ToString() == field.ToString();
                }
                Transmission.Add(new Field()
                {
                    Name = field.ToString().Substring(0, 1).ToUpper() + field.ToString().Substring(1),
                    IsChecked = IsChecked.ToString()
                });
            }
            Engine = new List<Field>();
            foreach (var field in availableFilters.Engine)
            {
                bool IsChecked = false;
                if (filter.Engine != null)
                {
                    IsChecked = filter.Engine.ToString() == field.ToString();
                }
                Engine.Add(new Field()
                {
                    Name = field.ToString().Substring(0, 1).ToUpper() + field.ToString().Substring(1),
                    IsChecked = IsChecked.ToString()
                });
            }
            Drive = new List<Field>();
            foreach (var field in availableFilters.Drive)
            {
                bool IsChecked = false;
                if (filter.Drive != null)
                {
                    IsChecked = filter.Drive.ToString() == field.ToString();
                }
                Drive.Add(new Field()
                {
                    Name = field.ToString().Substring(0, 1).ToUpper() + field.ToString().Substring(1),
                    IsChecked = IsChecked.ToString()
                });
            }

        }
        public AvailableFiltersForView() { }
    }
}
