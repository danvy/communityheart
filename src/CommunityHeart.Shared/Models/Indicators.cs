using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CommunityHeart.Models
{
    [DataContract]
    public class Indicators
    {
        [DataMember(Name="heartRate")]
        public int HeartRate { get; set; }
        [DataMember(Name = "heartRateIndicator")]
        public HeartRateIndicator HeartRateIndicator { get; set; }
    }
}
