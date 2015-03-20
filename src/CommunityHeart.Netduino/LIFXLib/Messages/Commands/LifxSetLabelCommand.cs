﻿using System;
#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)

#else
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endif


namespace LifxLib.Messages
{
    public class LifxSetLabelCommand : LifxCommand
    {
        private const UInt16 PACKET_TYPE = 0x18;
        private string mLabelName = "";

       
        public LifxSetLabelCommand(string newLabelName)
            : base(PACKET_TYPE, null)
        {
            mLabelName = newLabelName;    
        }

        public override byte[] GetRawMessage()
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(mLabelName);

            byte[] payload = new byte[32];

            

            Array.Copy(bytes, payload, Math.Min(payload.Length,bytes.Length));

            return payload;
        }


        public string LabelName
        {
            get { return mLabelName; }
            set { mLabelName = value; }
        }
    }
}
