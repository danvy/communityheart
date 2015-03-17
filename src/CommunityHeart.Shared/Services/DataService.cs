using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using CommunityHeart.Tools;
using CommunityHeart.Models;
using System.Threading.Tasks;

namespace CommunityHeart.Services
{
    public class DataService : IDataService
    {
        private string _serverUrl = "http://demoiotcommu.azure-mobile.net/api/";
        public async Task<bool> SetConfigAsync(DeviceConfig config)
        {
            var client = new HttpClient();
            var json = Json.Serialize(config);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(_serverUrl + "setconfig", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> SetValuesAsync(DeviceValues data)
        {
            var client = new HttpClient();
            var json = Json.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(_serverUrl + "setvalues", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<Indicators> GetIndicatorsAsync()
        {
            var client = new HttpClient();
            var content = await client.GetStringAsync(_serverUrl + "getindicators");
            return Json.Deserialize<Indicators>(content);
        }
    }
}
