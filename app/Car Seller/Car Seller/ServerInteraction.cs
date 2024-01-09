using Car_Seller.models;
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

namespace Car_Seller
{
    internal class ServerInteraction
    {
        private static string HOST = "http://192.168.0.131";
        private static string PORT = "8000";
        private static HttpClient client = new HttpClient{BaseAddress = new Uri(HOST + ":" + PORT), Timeout = TimeSpan.FromMilliseconds(1000) };
        private static CancellationTokenSource cts = new CancellationTokenSource();
        public static async Task Ping()
        {

            try
            {
                HttpResponseMessage response = await client.GetAsync("/ping", cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Неудачный HTTP-статус: {response.StatusCode}");
                }
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
        }
    
        public static async Task<AvailableFilters> GetAvailableFilters(Filter filter)
        {
            string urlPatametrs = filter.GetUrlParams();
            try
            {
                HttpResponseMessage response = await client.GetAsync("/car/available_filters?"+urlPatametrs, cts.Token);
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
                return new AvailableFilters();
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Ошибка соединения с сервером");
            }
        }
    }
}
