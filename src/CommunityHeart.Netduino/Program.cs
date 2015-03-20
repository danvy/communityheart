using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using System.Collections;

// Third Party
using Json.NETMF;

// Inner 
using CommunityHeart.Netduino.HTTP;
using CommunityHeart.Netduino.LIFX;
using CommunityHeart.Netduino.RGB;


namespace CommunityHeart.Netduino
{
    public class Program
    {
        public static void Main()
        {
            //int CRITICAL_RATE = 130;
            NetworkInterface ni;
            // Wait for DHCP (on LWIP devices)
            while (true)
            {
                ni = NetworkInterface.GetAllNetworkInterfaces()[0];

                if (ni.IPAddress != "0.0.0.0") break;

                Thread.Sleep(1000);
            }


            // Data Handler
#if USE_DUMMYHTTP
            DummyDataHandler dataHandler = new DummyDataHandler();
#else

            HTTPRequestHandler dataHandler = new HTTPRequestHandler("http://demoiotcommu.azure-mobile.net/api/getindicators");
#endif
            dataHandler.Initialize();

            // Light
#if USE_DUMMYLIGHT
            DummyLight light = new DummyLight();
#else
            LIFXLight light = new LIFXLight();
#endif
            light.Initialize();

            // RGB Gradiant
            RGBGradient rgbGradient = new RGBGradient(new RGB.RGB((byte)0, (byte)255, (byte)0), new RGB.RGB((byte)255, (byte)0, (byte)0), 30, 180);

            do
            {
                string data = dataHandler.Data;
                Debug.Print(data);

                Hashtable dataValues = JsonSerializer.DeserializeString(data) as Hashtable;
                long heartRate = 60;


                try
                {
                    heartRate = (long)dataValues["heartRate"];
                    Debug.Print("Heart Rate = " + heartRate.ToString());

                }
                catch (Exception e)
                {
                    Debug.Print("JSON data miss formed");

                }

                RGB.RGB color = rgbGradient.fromValue((int)heartRate);

                try
                {
                    light.SetColor(color.R, color.G, color.B);
                }
                catch (Exception ex)
                {
                    Debug.Print("Unable to set Light Color");
                }

            } while (true);

        }

    }
}
