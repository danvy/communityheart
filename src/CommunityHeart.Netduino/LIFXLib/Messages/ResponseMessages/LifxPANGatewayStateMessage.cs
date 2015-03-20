﻿using System;
#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)

#else
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endif

namespace LifxLib.Messages
{
    public class LifxPANGatewayStateMessage : LifxReceivedMessage
    {
        public const UInt16 PACKET_TYPE = 0x03;

        public LifxPANGatewayStateMessage()
            : base(PACKET_TYPE)
        {

        }


        public LifxPANServiceType ServiceType
        {
            get
            {
                return (LifxPANServiceType)base.ReceivedData.Payload[0];
            }
        }


        public UInt32 Port
        {
            get
            {
                return BitConverter.ToUInt32(ReceivedData.Payload, 1);
            }
            set
            {
                byte[] bytes = BitConverter.GetBytes(value);

                Array.Copy(bytes, 0, ReceivedData.Payload, 1, bytes.Length);
            }
        }
    }
}
