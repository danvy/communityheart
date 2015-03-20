﻿using System;
#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
using System.Collections;
#else
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endif
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LifxLib.Messages;



namespace LifxLib
{
    public class LifxCommunicator : IDisposable
    {
        public class UdpState
        {
            public UdpClient udpClient;
            public IPEndPoint endPoint;
        }
        public class IncomingMessage
        { 
            public LifxDataPacket Data;
            public IPEndPoint BulbAddress;

            public IncomingMessage(LifxDataPacket data, IPEndPoint address)
            {
                Data = data;
                BulbAddress = address;
            }
        }

#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
        private Queue mIncomingQueue = new Queue();
        private ArrayList mFoundPanHandlers = new ArrayList();
#else
        private Queue<IncomingMessage> mIncomingQueue = new Queue<IncomingMessage>(10);

        private List<LifxPanController> mFoundPanHandlers = new List<LifxPanController>();
#endif
        private const Int32 LIFX_PORT = 56700;
        private int mTimeoutMilliseconds = 1000;
        private UdpClient mSendCommandClient;
        private UdpClient mListnerClient;
        private bool mIsInitialized = false;
        private bool mIsDisposed = false;
        private static LifxCommunicator mInstance  = new LifxCommunicator();

        public static LifxCommunicator Instance
        {
            get
            {
                return mInstance;
            }
        }
        public int TimeoutMilliseconds
        {
            get { return mTimeoutMilliseconds; }
            set { mTimeoutMilliseconds = value; }
        }

        private LifxCommunicator()
        {

            
        }

        /// <summary>
        /// Initializes the listner for bulb messages
        /// </summary>
        public void Initialize()
        {
            IPEndPoint end = new IPEndPoint(IPAddress.Any, 56700);
            mListnerClient = new UdpClient(end);
#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
            mListnerClient.Blocking = false;
#else 
            mListnerClient.Client.Blocking = false;
#endif
            UdpState udpState = new UdpState();
            udpState.endPoint = end;
            udpState.udpClient = mListnerClient;
#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
            mListnerClient.SetSocketOption(
                    SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
#else
            mListnerClient.Client.SetSocketOption(
                SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
#endif

            // DO NOT NEED RESPONSE FROM THE LIFX
            //mListnerClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpState);
            mIsInitialized = true;
        }

#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
        private void ReceiveCallback(IAsyncResult ar)
        {
            ReceiveCallback((UDPAsyncResult)ar);
        }

        private void ReceiveCallback(UDPAsyncResult ar)
#else
        private void ReceiveCallback(IAsyncResult ar)
#endif
        {
            if (mIsDisposed)
                return;
           
            UdpClient client = (UdpClient)((UdpState)(ar.AsyncState)).udpClient;
            IPEndPoint endPoint = (IPEndPoint)((UdpState)(ar.AsyncState)).endPoint;            

            Byte[] receiveBytes = client.EndReceive(ar, ref endPoint);
            string receiveString = LifxHelper.ByteArrayToString(receiveBytes);

            LifxDataPacket package = new LifxDataPacket(receiveBytes);

            mIncomingQueue.Enqueue(new IncomingMessage(package, endPoint));

            client.BeginReceive(new AsyncCallback(ReceiveCallback), (UdpState)(ar.AsyncState));
            
        }

        /// <summary>
        /// Discovers the PanControllers (including their bulbs)
        /// </summary>
        /// <returns>List of bulbs</returns>
#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
        public ArrayList Discover()
#else
        public List<LifxPanController> Discover()
#endif
        {
            LifxGetPANGatewayCommand getPANCommand = new LifxGetPANGatewayCommand();

           mFoundPanHandlers.Clear();
           mTimeoutMilliseconds = 1500;
           int savedTimeout = mTimeoutMilliseconds;

           try
           {

               SendCommand(getPANCommand, LifxPanController.UninitializedPanController);

               foreach (LifxPanController controller in mFoundPanHandlers)
               {
                   LifxGetLightStatusCommand getBulbs = new LifxGetLightStatusCommand();
                   getBulbs.IsDiscoveryCommand = true;

                   SendCommand(getBulbs, controller);
               
               }

               return mFoundPanHandlers;
           }
           catch (Exception e)
           {
               //In case of any other exception, re-throw
               throw e;
           }
           finally 
           {
               mTimeoutMilliseconds = savedTimeout;
           }
            
            
        }

        public LifxReceivedMessage SendCommand(LifxCommand command, LifxPanController panController)
        {
            return SendCommand(command, "", panController.MacAddress, panController.IpEndpoint);
        }

        /// <summary>
        /// Sends command to a bulb
        /// </summary>
        /// <param name="command"></param>
        /// <param name="bulb">The bulb to send the command to.</param>
        /// <returns>Returns the response message. If the command does not trigger a response it will reurn null. </returns>
        public LifxReceivedMessage SendCommand(LifxCommand command, string macAddress, string panController, IPEndPoint endPoint)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("The communicator needs to be initialized before sending a command.");


            UdpClient client = GetConnectedClient(command, endPoint);


            LifxDataPacket packet = new LifxDataPacket(command);
            packet.TargetMac = LifxHelper.StringToByteArray(macAddress);
            packet.PanControllerMac = LifxHelper.StringToByteArray(""); //PanControllerMac has to be set to 0.0.0.0 to commincate with the LIFX

            client.Send(packet.PacketData, packet.PacketData.Length);

            DateTime commandSentTime = DateTime.Now;

            if (command.ReturnMessage == null)
                return null;

#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
            // Timespan.Ticks is expressed in 100 nanoseconds
            while (((DateTime.Now - commandSentTime).Ticks / 10000) < mTimeoutMilliseconds)
#else
            while ((DateTime.Now - commandSentTime).TotalMilliseconds < mTimeoutMilliseconds)
#endif
            {
                if (mIncomingQueue.Count != 0)
                {
                    IncomingMessage mess = (IncomingMessage)mIncomingQueue.Dequeue();
                    LifxDataPacket receivedPacket = mess.Data;


                    if (receivedPacket.PacketType == LifxPANGatewayStateMessage.PACKET_TYPE) 
                    { 
                        //Panhandler identified
                        LifxPANGatewayStateMessage panGateway = new LifxPANGatewayStateMessage();
                        panGateway.ReceivedData = receivedPacket;

                        AddDiscoveredPanHandler(new LifxPanController(
                               LifxHelper.ByteArrayToString(receivedPacket.TargetMac),
                               mess.BulbAddress));

                    }
                    else if (receivedPacket.PacketType == LifxLightStatusMessage.PACKET_TYPE && command.IsDiscoveryCommand)
                    {
                        //Panhandler identified
                        LifxLightStatusMessage panGateway = new LifxLightStatusMessage();
                        panGateway.ReceivedData = receivedPacket;

                    }
                    else if (receivedPacket.PacketType == command.ReturnMessage.PacketType)
                    {
                       
                        command.ReturnMessage.ReceivedData = receivedPacket;
                        mIncomingQueue.Clear();
                        return command.ReturnMessage;
                    }
                }
                Thread.Sleep(30);
            }

            if (command.IsDiscoveryCommand)
                return null;

            if (command.RetryCount > 0)
            {
                command.RetryCount -= 1;

                //Recurssion
                return SendCommand(command, macAddress, panController, endPoint);
            }
            else
                throw new TimeoutException("Did not get a reply from bulb in a timely fashion");

        }


        private void AddDiscoveredPanHandler(LifxPanController foundPanHandler)
        {
            foreach (LifxPanController handler in mFoundPanHandlers)
            {
                if (handler.MacAddress == foundPanHandler.MacAddress)
                    return;//already added
            }

            mFoundPanHandlers.Add(foundPanHandler);
        }

        private UdpClient GetConnectedClient(LifxCommand command, IPEndPoint endPoint)
        {
            if (mSendCommandClient == null)
            {
                return CreateClient(command, endPoint);
            }
            else
            { 
                if (command.IsBroadcastCommand)
                {
#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
                    if (mSendCommandClient.EnableBroadcast)
#else
                    if (mSendCommandClient.Client.EnableBroadcast)
#endif
                    {
                        return mSendCommandClient;
                    }
                    else
                    {
                        mSendCommandClient.Close();
                        return CreateClient(command, endPoint);
                    }
                }
                else
                {
#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
                    if (mSendCommandClient.EnableBroadcast)
#else
                    if (mSendCommandClient.Client.EnableBroadcast)
#endif
                    {
                        mSendCommandClient.Close();
                        return CreateClient(command, endPoint); 
                        
                    }
                    else
                    {
                        return mSendCommandClient;
                    }
                }
            }
        }

        private UdpClient CreateClient(LifxCommand command, IPEndPoint endPoint)
        {
            if (command.IsBroadcastCommand)
            {
                mSendCommandClient = new UdpClient();

                mSendCommandClient.EnableBroadcast = true;
#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
                mSendCommandClient.SetSocketOption(
                        SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                mSendCommandClient.Connect(new IPEndPoint(IPAddress.Parse("255.255.255.255"), LIFX_PORT));

#else
                mSendCommandClient.Client.SetSocketOption(
                SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                mSendCommandClient.Connect(new IPEndPoint(IPAddress.Broadcast, LIFX_PORT));
#endif
                return mSendCommandClient;
            }
            else
            {
                mSendCommandClient = new UdpClient();

#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
                mSendCommandClient.SetSocketOption(
                SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
#else
                mSendCommandClient.Client.SetSocketOption(
                SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
#endif
                mSendCommandClient.Connect(endPoint);
                return mSendCommandClient;

            }
        }

        public bool IsInitialized
        {
            get { return mIsInitialized; }
            set { mIsInitialized = value; }
        }

        public void CloseConnections()
        {
            mListnerClient.Close();
            mSendCommandClient.Close();
        }

        #region IDisposable Members

        public void Dispose()
        {
            mIsDisposed = true;
            CloseConnections();
        }

        #endregion
    }
}
