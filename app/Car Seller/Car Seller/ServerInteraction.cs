using System;
using System.Collections.Generic;
using System.Net.Http;
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
    }
}
