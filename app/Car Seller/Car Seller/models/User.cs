using System;
using System.Collections.Generic;
using System.Text;

namespace Car_Seller.models
{
    internal class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public static User FromDict(Dictionary<string, object> dict)
        {
            User result = new User();
            result.Id = int.Parse(dict["id"].ToString());
            result.Name = dict["name"].ToString();
            result.Surname = dict["surname"].ToString();
            result.Email = dict["email"].ToString();
            result.PhoneNumber = dict["phone_number"].ToString();
            return result;
        }
    }
}
