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

        public async Task<MyCookie> GetCookieAsync()
        {
            var tmp = await _connection.Table<MyCookie>().ToListAsync();

            if (tmp.Count == 0)
            {
                return null;
            }
            MyCookie cookie = tmp[0];
            if (cookie.Cookie.Length == 0)
            {
                return null;
            }

            return cookie;
        }

        public async Task<string> GetJWTAsync() 
        {
            var tmp = await _connection.Table<MyCookie>().ToListAsync();

            if (tmp.Count == 0) 
            {
                return "";
            }
            MyCookie cookie = tmp[0];
            if (cookie.Cookie.Length == 0) 
            {
                return "";
            }
            
            return cookie.Cookie;
            
        }
    
        public async Task SaveJWTAsync(string cookie)
        {
            MyCookie currentCookie = await GetCookieAsync();
            if (currentCookie != null)
            {
                await _connection.DeleteAsync<MyCookie>(currentCookie.Id);
            }
            MyCookie newCokie = new MyCookie();
            newCokie.Cookie = cookie;
            await _connection.InsertAsync(newCokie);
        }
    }
}
