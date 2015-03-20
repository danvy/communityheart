using System;
using Microsoft.SPOT;
using System.Collections;
using Json.NETMF;

namespace CommunityHeart.Netduino.HTTP
{
    public class DummyDataHandler: IDataHandler
    {

        JsonSerializer _serializer = new JsonSerializer(DateTimeFormat.Default);
        Hashtable _data = new Hashtable();
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Should be {"heartRate":50,"heartRateIndicator":1}</remarks>
        public DummyDataHandler()
        {
            byte heart = 60;
            _data.Add("heartRate", heart);
            byte indicator = 1;
            _data.Add("heartRateIndicator", indicator);
        }

        public string Data
        {
            get 
            {
                return _serializer.Serialize(_data);
            }
        }

        public bool Initialize()
        {
            return true;
        }
    }
}
