using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommunityHeart.Models
{
    [DataContract]
    public class DeviceValues
    {
        [DataMember(Name="heartRate")]
        public int HeartRate { get; set; }
    }
}
