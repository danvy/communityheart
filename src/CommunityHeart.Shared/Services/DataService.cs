using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Windows.Data.Json;
using CommunityHeart.Tools;
using CommunityHeart.Models;
using System.Threading.Tasks;

namespace CommunityHeart.Services
{
    public class DataService : IDataService
    {
        private string _serverUrl = "";
        public async Task InitAsync(Models.DeviceInit init)
        {
            var client = new HttpClient();
            var json = Json.Serialize(init);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync(_serverUrl, content);
        }
        public async Task SendDataAsync(Models.DeviceData data)
        {
            var client = new HttpClient();
            var json = Json.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync(_serverUrl, content);
        }
        public async Task<Indicators> GetIndicatorsAsync()
        {
            var client = new HttpClient();
            var content = await client.GetStringAsync(_serverUrl);
            return Json.Deserialize<Indicators>(content);
        }
    }
}
