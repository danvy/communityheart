using System;
using Microsoft.SPOT;

namespace CommunityHeart.Netduino.LIFX
{
    public class DummyLight: ILight
    {
        public bool SetColor(byte r, byte g, byte b)
        {
            return true;
        }

        public bool SetDimming(double percent)
        {
            return true;
        }

        public bool Initialize()
        {
            return true;
        }
    }
}
