﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RouterVpnManagerClientLibrary
{
    public class RouterVpnManagerConnection : IDisposable
    {
        private TcpClient client_;


        public delegate void Callback(JObject message);

        public RouterVpnManagerConnection()
        {
            client_ = new TcpClient();
            SetDefaults();
        }

        public bool Connect()
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Host), Port);
                client_.Connect(ep);

                JObject obj = ControlledRequests.FormatMessage("request","connection");
                byte[] bytes = Encoding.ASCII.GetBytes(obj.ToString());

                NetworkStream ns = client_.GetStream();
                ns.Write(bytes, 0, bytes.Length);

                ProcessCallback((JObject message) =>
                {
                    RouterVpnManagerLogLibrary.Log(message["data"].ToString());
                });


                return true;
            }
            catch (Exception e)
            {
                RouterVpnManagerLogLibrary.Log(e.ToString());
                return false;
            }

        }


        private bool ProcessCallback(Callback callback)
        {
            try
            {
                NetworkStream ns = client_.GetStream();
                byte[] bytesResponse = new byte[client_.ReceiveBufferSize];
                int bytesRead = ns.Read(bytesResponse, 0, client_.ReceiveBufferSize);
                string dataReceived = Encoding.ASCII.GetString(bytesResponse, 0, bytesRead);
                JObject obj = JObject.Parse(dataReceived);
                callback(obj);
                return true;
            }
            catch (Exception e)
            {
                RouterVpnManagerLogLibrary.Log(e.ToString());
                return false;
            }

        }


        public void Disconnect()
        {
            client_.Close();
        }

        //public bool SendPlainTextMessage(string text, Callback callback = null)
        //{
        //    if (client_.Connected)
        //    {
        //        NetworkStream ns = client_.GetStream();
        //        byte[] bytes = Encoding.ASCII.GetBytes(text);
        //        ns.Write(bytes, 0, bytes.Length);
        //        if (callback != null)
        //        {
        //            ProcessCallback(callback);
        //        }
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public bool SendJson(JObject obj, Callback callback = null)
        {
            if (client_.Connected)
            {
                NetworkStream ns = client_.GetStream();
                byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
                ns.Write(bytes, 0, bytes.Length);
                if (callback != null)
                {
                    ProcessCallback(callback);
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        public void Dispose()
        {
            if (client_.Connected)
            {
                Disconnect();
            }
        }

        private void SetDefaults()
        {
            SendTimeout = Properties.Settings.Default.Timeout;
            RecivedTimeout = Properties.Settings.Default.Timeout;
            Host = Properties.Settings.Default.Host;
            Port = Properties.Settings.Default.Port;
        }

        public int SendTimeout { get => client_.SendTimeout; set => client_.SendTimeout = value; }
        public int RecivedTimeout { get => client_.ReceiveTimeout; set => client_.ReceiveTimeout = value; }

        public string Host { get; set; }

        public int Port { get; set; }

    }
}
