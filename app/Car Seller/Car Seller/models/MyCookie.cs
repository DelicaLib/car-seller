using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Car_Seller.models
{
    public class MyCookie
    {
        [PrimaryKey, AutoIncrement] 
        public int Id { get; set; }
        public string Cookie { get; set; }
    }
}
