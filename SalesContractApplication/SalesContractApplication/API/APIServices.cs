using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SalesContractApplication.Models;
using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SalesContractApplication.API
{
    public class APIServices
    {
        private readonly HttpClient _httpClient;

        public APIServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        private async Task<ApiResponseModel> GetJsonString(HttpResponseMessage response)
        {
            string result = "";
            var rp = new ApiResponseModel();
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    rp.Success = true;
                    rp.Data = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    rp.Success = false;
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    JObject json = JObject.Parse(jsonResponse);
                    if (json !=null && json["Error"] != null)
                    {
                        rp.Error = json["Error"].ToString();
                    }
                }
                result = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                rp.Error = $"[Error] {ex.Message}";
            }

            return rp;
        }


        public async Task<bool> IsServerReachable(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string host = uri.Host;

                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(host);

                    // If the ping is successful, return true
                    return reply.Status == IPStatus.Success;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., invalid URL, DNS resolution failure)
                Console.WriteLine($"Ping failed: {ex.Message}");
                return false;
            }
        }


        public async Task<ApiResponseModel> SendPostRequest(string url, string jsonData, string token = "")
        {
            if (!await IsServerReachable(url))
            {
                return new ApiResponseModel { Success = false, Error = "API server is unreachable (Ping failed)" };
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
                };
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Add("Authorization", token);
                }
                var response = await _httpClient.SendAsync(request);
                return await GetJsonString(response);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResponseModel { Success = false, Error = "API server is down or unreachable: " + ex.Message };
            }
        }

        public async Task<ApiResponseModel> SendGetRequest(string url, string jsonData, string token)
        {
            if (!await IsServerReachable(url))
            {
                return new ApiResponseModel { Success = false, Error = "API server is unreachable (Ping failed)" };
            }

            try
            {
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                    var query = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
                    url = $"{url}?{query}";
                }

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", token);

                var response = await _httpClient.SendAsync(request);
                return await GetJsonString(response);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResponseModel { Success = false, Error = "API server is down or unreachable: " + ex.Message };
            }
        }
    }
}