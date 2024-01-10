using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Car_Seller.models;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Essentials;

namespace Car_Seller.services
{
    public class CookieDB
    {
        readonly SQLiteAsyncConnection _connection;

        public CookieDB(string connectionString)
        {
            _connection = new SQLiteAsyncConnection(connectionString);
            
            _connection.CreateTableAsync<MyCookie>().Wait();

        }

        public async Task<int> GetJWTAsync() 
        {
            var tmp = await _connection.Table<MyCookie>().ToListAsync();

            if (tmp.Count == 0) 
            {
                return -1;
            }
            MyCookie cookie = tmp[0];
            if (cookie.Cookie.Length == 0) 
            {
                return -1;
            }
            try
            {
                Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(cookie.Cookie);
                if (result.TryGetValue("JWT_token", out string jwt))
                {
                    return cookie.Id;
                }
                return -1;
            }
            catch (Exception)
            {

                return -1;
            }
        }
    
        public async Task SaveJWTAsync(string cookie)
        {
            int currentCookieId = await GetJWTAsync();
            if (currentCookieId != -1)
            {
                await _connection.DeleteAsync<MyCookie>(currentCookieId);
            }
            MyCookie newCokie = new MyCookie();
            newCokie.Cookie = cookie;
            await _connection.InsertAsync(newCokie);
        }
    }
}
