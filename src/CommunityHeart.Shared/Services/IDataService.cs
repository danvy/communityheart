using CommunityHeart.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunityHeart.Services
{
    public interface IDataService
    {
        Task InitAsync(DeviceInit init);
        Task SendDataAsync(DeviceData data);
        Task<Indicators> GetIndicatorsAsync();
    }
}
