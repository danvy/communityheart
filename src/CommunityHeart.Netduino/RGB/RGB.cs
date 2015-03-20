using System;
using Microsoft.SPOT;

namespace CommunityHeart.Netduino.RGB
{
    /// <summary>
    /// RGB manipulation object
    /// </summary>
    public class RGB
    {
        public byte R { set; get; }
        public byte G { set; get; }
        public byte B { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Red">Red</param>
        /// <param name="Green">Green</param>
        /// <param name="Blue">Blue</param>
        public RGB(byte Red, byte Green, byte Blue)
        {
            this.R = Red;
            this.G = Green;
            this.B = Blue;
        }

        /// <summary>
        /// Convertion to .Net MicroFramework color model
        /// </summary>
        public Microsoft.SPOT.Presentation.Media.Color Color
        {
            get
            {
                return Microsoft.SPOT.Presentation.Media.ColorUtility.ColorFromRGB(R, G, B);
            }

            set
            {
                Microsoft.SPOT.Presentation.Media.Color color = (Microsoft.SPOT.Presentation.Media.Color)value;
                R = Microsoft.SPOT.Presentation.Media.ColorUtility.GetRValue(color);
                G = Microsoft.SPOT.Presentation.Media.ColorUtility.GetGValue(color);
                B = Microsoft.SPOT.Presentation.Media.ColorUtility.GetBValue(color);
            }
        }
    }
}
