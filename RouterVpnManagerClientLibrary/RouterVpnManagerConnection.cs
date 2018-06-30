using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RouterVpnManagerClientLibrary
{
    public class RouterVpnManagerConnection : IDisposable
    {
        private RequestProcessor requestProcessor_;
        private TcpClient client_;

        public RouterVpnManagerConnection()
        {
            client_ = new TcpClient();
            requestProcessor_ = new RequestProcessor(client_);
            SetDefaults();
        }

        public async Task<bool> Connect()
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Host), Port);
                client_.Connect(ep);

                requestProcessor_.Start();

                JObject obj = ControlledRequests.FormatMessage("request","connection");
                obj["signature"] = Guid.NewGuid().ToString();
                byte[] bytes = Encoding.ASCII.GetBytes(obj.ToString());

                Task<bool> callbackstate = AddCallback(obj, (JObject message) =>
                {
                    RouterVpnManagerLogLibrary.Log(message["data"].ToString());
                    return true;
                });
                NetworkStream ns = client_.GetStream();
                ns.Write(bytes, 0, bytes.Length);

                return await callbackstate;
            }
            catch (Exception e)
            {
                RouterVpnManagerLogLibrary.Log(e.ToString());
                return false;
            }

        }


        private Task<bool> AddCallback(JObject response,RequestProcessor.Callback callback)
        {
            ManualResetEvent oSignalEvent = new ManualResetEvent(false);
            bool responseState = true;
            responseState = requestProcessor_.AddPrivateCallbackHandler(response, callback, oSignalEvent);
            return Task.Run(() =>
            {
                try
                {
                    oSignalEvent.WaitOne(Properties.Settings.Default.Timeout);
                    return responseState;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            });
        }


        public void Disconnect()
        {
            requestProcessor_.Stop();
            client_.Close();
        }

        public async Task<bool> SendJson(JObject obj, RequestProcessor.Callback callback = null)
        {
            JObject o = (JObject)obj.DeepClone();
            if (client_.Connected)
            {
                o["signature"] = Guid.NewGuid().ToString();

                NetworkStream ns = client_.GetStream();
                byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(o));
                ns.Write(bytes, 0, bytes.Length);
                if (callback != null)
                {
                    return await AddCallback(o,callback);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddBroadcastCallbackHandler(string request, RequestProcessor.Callback callback)
        {
            return requestProcessor_.AddPublicCallbackHandler(request, callback);
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
            RecivedTimeout = 0;
            Host = Properties.Settings.Default.Host;
            Port = Properties.Settings.Default.Port;
        }

        public int SendTimeout { get => client_.SendTimeout; set => client_.SendTimeout = value; }
        public int RecivedTimeout { get => client_.ReceiveTimeout; set => client_.ReceiveTimeout = value; }

        public string Host { get; set; }

        public int Port { get; set; }

    }
}
