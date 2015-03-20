using System;
using Microsoft.SPOT;

namespace CommunityHeart.Netduino.LIFX
{
    interface ILight
    {
        bool Initialize();
        bool SetColor(byte r, byte g, byte b);
        bool SetDimming(double percent);
    }
}
