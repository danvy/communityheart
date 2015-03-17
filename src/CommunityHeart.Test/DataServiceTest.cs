using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityHeart.Services;
using CommunityHeart.Tools;
using CommunityHeart.Models;
using System.Threading.Tasks;

namespace CommunityHeart.Test
{
    [TestClass]
    public class DataServiceTest
    {
        [TestInitialize]
        public void Initialize()
        {
            IoC.Instance.Register<IDataService>(new DataService());
        }
        [TestMethod]
        public async Task SetConfig()
        {
            var service = IoC.Instance.Resolve<IDataService>();
            var config = new DeviceConfig()
            {
                HeartRateMin = 50,
                HeartRateMax = 100
            };
            Assert.IsTrue(await service.SetConfigAsync(config));
        }
        [TestMethod]
        public async Task SetValues()
        {
            var service = IoC.Instance.Resolve<IDataService>();
            var values = new DeviceValues()
            {
                HeartRate = 50
            };
            Assert.IsTrue(await service.SetValuesAsync(values));
        }
        [TestMethod]
        public async Task SetValuesMultiple()
        {
            for(var i = 0; i < 10; i++)
            {
                await SetValues();
            }
        }
        [TestMethod]
        public async Task GetIndicators()
        {
            var service = IoC.Instance.Resolve<IDataService>();
            var indicators = await service.GetIndicatorsAsync();
            Assert.IsFalse(indicators.HeartRate == 0);
        }
    }
}
