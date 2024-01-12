using Car_Seller.models;
using Car_Seller.services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Car_Seller
{
    internal class ServerInteraction
    {
        private static string HOST = "http://172.16.9.218";
        private static string PORT = "8000";
        private static HttpClient client = new HttpClient { BaseAddress = new Uri(HOST + ":" + PORT), Timeout = TimeSpan.FromMilliseconds(1000) };
        private static CancellationTokenSource cts = new CancellationTokenSource();
        private static CookieDB cookieDB = App.CookieDBObj;

        public static string GetURL()
        {
            return HOST + ":" + PORT + "/";
        }
        public static async Task Ping()
        {

            try
            {
                HttpResponseMessage response = await client.GetAsync("/ping", cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Неудачный HTTP-статус: {response.StatusCode}");
                }
                return;
            }
            catch (SocketException ex)
            {
                throw new HttpRequestException("Ошибка соединения с сервером");
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    // a real cancellation, triggered by the caller
                }
                else
                {
                    throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
                }
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            return;
        }

        public static async Task<AvailableFilters> GetAvailableFilters(Filter filter)
        {
            string urlPatametrs = filter.GetUrlParams();
            try
            {
                HttpResponseMessage response = await client.GetAsync("/car/available_filters?" + urlPatametrs, cts.Token);
                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new WebException(content, (WebExceptionStatus)422);
                }
                AvailableFilters filters = JsonConvert.DeserializeObject<AvailableFilters>(content);
                return filters;

            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    return new AvailableFilters();
                }
                else
                {
                    throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
                }
            }
            catch (OperationCanceledException ex)
            {
                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Ошибка соединения с сервером");
            }
        }

        public static async Task<List<Car>> GetCarsAsync(Filter filter, int pageSize, int page)
        {
            string urlPatametrs = filter.GetUrlParams() + $"&page_size={pageSize}&page={page}";
            try
            {
                HttpResponseMessage response = await client.GetAsync("/car/catalog?" + urlPatametrs, cts.Token);
                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new WebException(content, (WebExceptionStatus)422);
                }
                List<Dictionary<string, object>> carsRaw = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content);

                List<Car> cars = new List<Car>();
                foreach (var item in carsRaw)
                {
                    cars.Add(new Car(item));
                }
                return cars;

            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    return new List<Car>();
                }
                else
                {
                    throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
                }
            }
            catch (OperationCanceledException ex)
            {
                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Ошибка соединения с сервером");
            }
        }

        public static async Task<bool> CarIsLiked(int CarId)
        {
            try
            {
                if (!await SetCookieinClient())
                {
                    return false;
                }
                HttpResponseMessage response = await client.GetAsync($"/like/car/{CarId}", cts.Token);
                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new WebException(content, (WebExceptionStatus)422);
                }
                bool tmp = JsonConvert.DeserializeObject<bool>(content);
                return tmp;

            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    return false;
                }
                else
                {
                    throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
                }
            }
            catch (OperationCanceledException ex)
            {
                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Ошибка соединения с сервером");
            }
        }

        public static async Task<bool> HasJWTAsync()
        {
            MyCookie jwtId = await cookieDB.GetCookieAsync();
            return jwtId != null;
        }

        public static async Task<bool> Login(string email, string password, string recaptcha)
        {
            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
            var loginData = new Dictionary<string, string>()
            {
                { "email", email },
                { "password", password }
            };

            var recaptchaData = new Dictionary<string, string>()
            {
                { "recaptcha_response", recaptcha }
            };

            var requestData = new Dictionary<string, Dictionary<string, string>>()
            {
                {"login_data", loginData },
                {"recaptcha", recaptchaData }
            };

            var requestContent = JsonConvert.SerializeObject(requestData);
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                HttpResponseMessage response = await client.PostAsync("/user/login", new StringContent(requestContent, Encoding.UTF8, "application/json"), cts.Token);
                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new WebException(content, (WebExceptionStatus)404);
                }
                var userData = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                var tmp = JsonConvert.DeserializeObject<Dictionary<string, string>>(userData["jwt_token"].ToString());
                await cookieDB.SaveJWTAsync(tmp["access_token"]);
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    return false;
                }
                else
                {
                    throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
                }
            }
            catch (OperationCanceledException ex)
            {
                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            catch (WebException ex)
            {
                if (ex.Status != (WebExceptionStatus)404)
                {
                    throw;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Ошибка соединения с сервером");
            }
        }

        public static async Task<bool> SetLike(int carId)
        {

            try
            {
                if (!await SetCookieinClient())
                {
                    throw new WebException("Вы не авторизованы", (WebExceptionStatus)401);
                }
                HttpResponseMessage response = await client.PostAsync($"like/car/{carId}", new StringContent(""), cts.Token);
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new WebException(content, (WebExceptionStatus)401);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new WebException(content, (WebExceptionStatus)400);
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {

                    return false;
                }
                else
                {

                    throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
                }
            }
            catch (OperationCanceledException ex)
            {

                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            catch (WebException ex)
            {

                throw;
            }
            catch (Exception ex)
            {

                throw new HttpRequestException("Ошибка соединения с сервером");
            }

        }

        public static async Task<bool> DeleteLike(int carId)
        {

            try
            {
                if (!await SetCookieinClient())
                {
                    throw new WebException("Вы не авторизованы", (WebExceptionStatus)401);
                }
                HttpResponseMessage response = await client.DeleteAsync($"like/car/{carId}", cts.Token);
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new WebException(content, (WebExceptionStatus)401);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new WebException(content, (WebExceptionStatus)400);
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {

                    return false;
                }
                else
                {

                    throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
                }
            }
            catch (OperationCanceledException ex)
            {

                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            catch (WebException ex)
            {

                throw;
            }
            catch (Exception ex)
            {

                throw new HttpRequestException("Ошибка соединения с сервером");
            }

        }

        public static async Task<bool> Logout()
        {
            try
            {
                if (!await SetCookieinClient())
                {
                    throw new WebException("Вы не авторизованы", (WebExceptionStatus)401);
                }
                HttpResponseMessage response = await client.PostAsync($"user/logout", new StringContent(""), cts.Token);
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await cookieDB.Clear();
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    await cookieDB.Clear();
                    throw new WebException(content, (WebExceptionStatus)400);
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {

                    return false;
                }
                else
                {

                    throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
                }
            }
            catch (OperationCanceledException ex)
            {

                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            catch (WebException ex)
            {

                throw;
            }
            catch (Exception ex)
            {

                throw new HttpRequestException("Ошибка соединения с сервером");
            }
        }

        public static async Task<User> GetProfileData()
        {
            try
            {
                if (!await SetCookieinClient())
                {
                    throw new WebException("Вы не авторизованы", (WebExceptionStatus)401);
                }
                HttpResponseMessage response = await client.GetAsync($"user/me/", cts.Token);
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    return User.FromDict(result);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await cookieDB.Clear();
                    throw new WebException(content, (WebExceptionStatus)401);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    await cookieDB.Clear();
                    throw new WebException(content, (WebExceptionStatus)404);
                }
                throw new HttpRequestException();
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            catch (OperationCanceledException ex)
            {

                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            catch (WebException ex)
            {

                throw;
            }
            catch (Exception ex)
            {

                throw new HttpRequestException("Ошибка соединения с сервером");
            }
        }

        public static async Task<bool> ChangeUserData(string phoneNumber, string email, string password, string recaptcha)
        {
            try
            {
                if (!await SetCookieinClient())
                {
                    throw new WebException("Вы не авторизованы", (WebExceptionStatus)401);
                }
                string requestBody = JsonConvert.SerializeObject(new Dictionary<string, Dictionary<string, string>>()
                    {
                        {"recaptcha", new Dictionary<string, string>()
                            {
                                {"recaptcha_response", recaptcha }
                            } 
                        },
                        {"user_new_data", new Dictionary<string, string>()
                            {
                                {"email", email },
                                {"phone_number", phoneNumber},
                                {"password", password}
                            }
                        }
                });

                HttpResponseMessage response = await client.PostAsync($"user/edit", new StringContent(requestBody, Encoding.UTF8, "application/json"), cts.Token);
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await cookieDB.Clear();
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new WebException(content, (WebExceptionStatus)400);
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new WebException(content, (WebExceptionStatus)403);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new WebException(content, (WebExceptionStatus)401);
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {

                    return false;
                }
                else
                {

                    throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
                }
            }
            catch (OperationCanceledException ex)
            {

                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            catch (WebException ex)
            {

                throw;
            }
            catch (Exception ex)
            {

                throw new HttpRequestException("Ошибка соединения с сервером");
            }
        }


        public static async Task<bool> Register(string name, string surname, string email, string phoneNumber, string password, string recaptcha)
        {
            var registerData = new Dictionary<string, string>()
            {
                { "name", name },
                { "surname", surname },
                { "email", email },
                { "password", password },
                { "phone_number", phoneNumber },
            };

            var recaptchaData = new Dictionary<string, string>()
            {
                { "recaptcha_response", recaptcha }
            };

            var requestData = new Dictionary<string, Dictionary<string, string>>()
            {
                {"new_user_data", registerData },
                {"recaptcha", recaptchaData }
            };

            var requestContent = JsonConvert.SerializeObject(requestData);
            try
            {
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.PostAsync("/user/new", new StringContent(requestContent, Encoding.UTF8, "application/json"), cts.Token);
                string content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new WebException(content, (WebExceptionStatus)400);
                }
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new WebException(content, (WebExceptionStatus)403);
                }
                var userData = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    return false;
                }
                else
                {
                    throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
                }
            }
            catch (OperationCanceledException ex)
            {
                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
            catch (WebException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Ошибка соединения с сервером");
            }
        }


        public static async Task<bool> SetCookieinClient()
        {
            string cookie = await cookieDB.GetJWTAsync();
            if (string.IsNullOrEmpty(cookie))
            {
                return false;
            }
            var cookieDict = JsonConvert.SerializeObject(new Dictionary<string, string>()
            {
                {"access_token", cookie}
            });
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri(GetURL()), new Cookie("jwt_token", cookieDict));
            try
            {
                if (client.DefaultRequestHeaders.Contains("Cookie"))
                {
                    client.DefaultRequestHeaders.Clear();
                }
                client.DefaultRequestHeaders.Add("Cookie", cookieContainer.GetCookieHeader(new Uri(GetURL())));
                return true;
            }
            catch (Exception)
            {
                throw new HttpRequestException("Ошибка при выполнении HTTP-запроса");
            }
        }
    }
}
