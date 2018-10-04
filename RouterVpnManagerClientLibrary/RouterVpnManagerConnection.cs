using System;
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

        public void Connect()
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Host), Port);
                client_.Connect(ep);
                

                requestProcessor_.Start();

                JObject obj = ControlledRequests.FormatMessage("request","connection");
                obj["signature"] = Guid.NewGuid().ToString();
                byte[] bytes = Encoding.ASCII.GetBytes(obj.ToString());

                bool state = false;
                HasCallbackBeenRecieved callbackstate = AddCallback(obj, (JObject message) =>
                {
                    RouterVpnManagerLogLibrary.Log(message["data"].ToString());
                    state = true;
                });
                NetworkStream ns = client_.GetStream();
                ns.Write(bytes, 0, bytes.Length);
                callbackstate.Wait(CallbackTimeout);
                if (!state)
                    throw new Exception("was not able to connect properly");
            }
            catch (Exception e)
            {
                RouterVpnManagerLogLibrary.Log(e.ToString());
                throw e;
            }

        }

        private HasCallbackBeenRecieved AddCallback(JObject response,RequestProcessor.Callback callback)
        {
            HasCallbackBeenRecieved callbackRecieved = new HasCallbackBeenRecieved();
            callbackRecieved.ResponseState = requestProcessor_.AddPrivateCallbackHandler(response, (JObject o) =>
            {
                callback(o);
                callbackRecieved.SignalEvent.Set();
            });

           callbackRecieved.SetupAsyncSignal();

            return callbackRecieved;
        }

        private HasCallbackBeenRecieved AddBroadcastCallback(JObject response)
        {
            HasCallbackBeenRecieved callbackRecieved = new HasCallbackBeenRecieved();
            callbackRecieved.ResponseState = requestProcessor_.AddBroadcastHandleListener(response, callbackRecieved);
            callbackRecieved.SetupAsyncSignal();
            return callbackRecieved;
        }


        public void DisconnectFromServer()
        {
            requestProcessor_.Stop();
            client_.Close();
        }

        /// <summary>
        /// Sends json to the server
        /// </summary>
        /// <param name="obj">The Json object</param>
        /// <param name="callback">A callback function which will be called when a response is recived</param>
        /// <param name="broadcastCallback">if it's a broadcast callback it will return a HasCallbackBeenRecieved which will trigger when the broadcast is recived</param>
        /// <returns></returns>
        public HasCallbackBeenRecieved SendJson(JObject obj, RequestProcessor.Callback callback = null,bool broadcastCallback = false)
        {
            HasCallbackBeenRecieved state = new HasCallbackBeenRecieved();
            JObject o = (JObject)obj.DeepClone();
            if (client_.Connected)
            {
                o["signature"] = Guid.NewGuid().ToString();

                NetworkStream ns = client_.GetStream();
                if (broadcastCallback)
                {
                    state = AddBroadcastCallback(o);

                }
                else if (callback != null)
                {
                    state = AddCallback(o, callback);
                }
                byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(o));
                ns.Write(bytes, 0, bytes.Length);
            }

            return state;
        }

        public bool AddBroadcastCallbackHandler(string request, RequestProcessor.Callback callback)
        {
            return requestProcessor_.AddPublicCallbackHandler(request, callback);
        }


        public void Dispose()
        {
            if (client_.Connected)
            {
                DisconnectFromServer();
            }
        }

        public bool IsConnected => client_.Connected;

        private void SetDefaults()
        {
            SendTimeout = 5000;
            RecivedTimeout = 10000;
            CallbackTimeout = 10000;
            Host = "127.0.0.1";
            Port = 8000;
        }
        /// <summary>
        /// How long it will wait untill a recieved ack is recived from the server
        /// </summary>
        public int SendTimeout { get => client_.SendTimeout; set => client_.SendTimeout = value; }
        /// <summary>
        /// The ReceiveTimeout property determines the amount of time that the Read method will block until it is able to receive data
        /// </summary>
        public int RecivedTimeout { get => client_.ReceiveTimeout; set => client_.ReceiveTimeout = value; }
        /// <summary>
        /// this is the total request time to send and recieve response json, parse it and call the callback function if applicable
        /// </summary>
        public int CallbackTimeout { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

    }
}
