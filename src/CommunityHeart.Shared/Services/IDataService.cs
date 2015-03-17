using CommunityHeart.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunityHeart.Services
{
    public interface IDataService
    {
        Task<bool> SetConfigAsync(DeviceConfig config);
        Task<bool> SetValuesAsync(DeviceValues values);
        Task<Indicators> GetIndicatorsAsync();
    }
}
