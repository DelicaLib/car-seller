using Car_Seller.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Car_Seller.viewModels
{
    internal class FilterPageViewModel : INotifyPropertyChanged
    {
        public AvailableFiltersForView availableFilters { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public FilterPageViewModel() 
        {
            availableFilters = new AvailableFiltersForView();
        }
        
    }
}
