using System;
using Microsoft.SPOT;

namespace CommunityHeart.Netduino.RGB
{
    /// <summary>
    /// Generate a gradient color
    /// </summary>
    public class RGBGradient
    {
        private RGB _startColor;
        private RGB _endColor;
        private int _valueMin;
        private int _valueMax;
        private double _rStep;
        private double _gStep;
        private double _bStep;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startColor">Gradient start color</param>
        /// <param name="endColor">Gradient end color</param>
        /// <param name="valueMin">Associated value to start color</param>
        /// <param name="valueMax">Associated value to end color</param>
        public RGBGradient(RGB startColor, RGB endColor, int valueMin, int valueMax)
        {
            _startColor = startColor;
            _endColor = endColor;
            _valueMax = valueMax;
            _valueMin = valueMin;

            _rStep = ((double)endColor.R - (double)startColor.R) / (_valueMax - _valueMin);
            _gStep = ((double)endColor.G - (double)startColor.G) / (_valueMax - _valueMin);
            _bStep = ((double)endColor.B - (double)startColor.B) / (_valueMax - _valueMin);
        }

        /// <summary>
        /// Retrieve a gradient color based on the value
        /// </summary>
        /// <param name="value">Value associated to the color to retrieve</param>
        /// <returns>Color associated to the requested value</returns>
        public RGB fromValue(int value)
        {
            if (value < _valueMin)
            {
                return _startColor;
            }
            else if (value > _valueMax)
            {
                return _endColor;
            }
            else
            {
                double steps = value - _valueMin;
                return new RGB(
                                (byte)(_startColor.R + (_rStep * steps)),
                                (byte)(_startColor.G + (_gStep * steps)),
                                (byte)(_startColor.B + (_bStep * steps))
                    );
            }
        }
    }
}
