using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using LifxLib;
using System.Net;

namespace CommunityHeart.Netduino.LIFX
{
    class LIFXLight: ILight
    {

        String LifxIPAdress = "10.16.0.24"; // HARD CODE
        int LifxPort = 56700;

        LifxPowerState currState = LifxPowerState.On;
        LifxPanController mPanController;

        public bool SetColor(byte r, byte g, byte b)
        {
            LifxColor mColor = new LifxColor(ColorUtility.ColorFromRGB(r,g,b), 50000);
            mPanController.SetColor(mColor, 0);
            return true;
        }

        public bool SetDimming(double percent)
        {
            throw new NotImplementedException();
        }

        // Toggle the LIFX if the rate is over critical_rate
        // Currently toggle with the API refreshment. TO DO : Handle in another thread. 
        public void CriticalRate(int rate, int critical_rate)
        {
            if (rate >= critical_rate)
            {
                if (currState == LifxPowerState.On)
                    currState = LifxPowerState.Off;
                else
                    currState = LifxPowerState.On;
            }
            else
                currState = LifxPowerState.On;
            mPanController.SetPowerState(currState);
        }

        public bool Initialize()
        {
            LifxCommunicator.Instance.Initialize();
            mPanController = new LifxPanController("", new IPEndPoint(IPAddress.Parse(LifxIPAdress), LifxPort));
            mPanController.SetPowerState(LifxPowerState.On);
            return true; //DO NOT RETURN ERROR...
        }
    }
}
