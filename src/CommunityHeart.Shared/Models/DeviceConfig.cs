using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommunityHeart.Models
{
    [DataContract]
    public class DeviceConfig
    {
        [DataMember(Name = "heartRateMax")]
        public int HeartRateMax { get; set; }
        [DataMember(Name = "heartRateMin")]
        public int HeartRateMin { get; set; }
    }
}
