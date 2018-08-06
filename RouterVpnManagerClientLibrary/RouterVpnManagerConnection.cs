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
                Task callbackstate = AddCallback(obj, (JObject message) =>
                {
                    RouterVpnManagerLogLibrary.Log(message["data"].ToString());
                    state = true;
                });
                NetworkStream ns = client_.GetStream();
                ns.Write(bytes, 0, bytes.Length);
                callbackstate.Wait();
                if (!state)
                    throw new Exception("was not able to connect properly");
            }
            catch (Exception e)
            {
                RouterVpnManagerLogLibrary.Log(e.ToString());
                throw e;
            }

        }


        private Task<bool> AddCallback(JObject response,RequestProcessor.Callback callback)
        {
            ManualResetEvent oSignalEvent = new ManualResetEvent(false);
            bool responseState = true;
            responseState = requestProcessor_.AddPrivateCallbackHandler(response, (JObject o) =>
            {
                callback(o);
                oSignalEvent.Set();
            });

            //https://stackoverflow.com/questions/18756354/wrapping-manualresetevent-as-awaitable-task
            //Most of this below is copy pasta black magic

            var tcs = new TaskCompletionSource<bool>();

            var registration = ThreadPool.RegisterWaitForSingleObject(oSignalEvent, (state, timedOut) =>
            {
                var localTcs = (TaskCompletionSource<bool>)state;
                if (timedOut)
                    localTcs.TrySetCanceled();
                else
                    localTcs.TrySetResult(responseState);
            },tcs, Timeout.InfiniteTimeSpan, executeOnlyOnce:true);

            tcs.Task.ContinueWith((_, state) => ((RegisteredWaitHandle)state).Unregister(null), registration, TaskScheduler.Default);
            return tcs.Task;
        }


        public void Disconnect()
        {
            requestProcessor_.Stop();
            client_.Close();
        }

        public async Task SendJson(JObject obj, RequestProcessor.Callback callback = null)
        {
            JObject o = (JObject)obj.DeepClone();
            if (client_.Connected)
            {
                o["signature"] = Guid.NewGuid().ToString();

                NetworkStream ns = client_.GetStream();
                if (callback != null)
                {
                    Task<bool> task = AddCallback(o, callback);
                    byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(o));
                    ns.Write(bytes, 0, bytes.Length);

                    await task;
                }
                else
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(o));
                    ns.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {

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

        public bool IsConnected => client_.Connected;

        private void SetDefaults()
        {
            SendTimeout = 5000;
            RecivedTimeout = 0;
            Host = "127.0.0.1";
            Port = 8000;
        }

        public int SendTimeout { get => client_.SendTimeout; set => client_.SendTimeout = value; }
        public int RecivedTimeout { get => client_.ReceiveTimeout; set => client_.ReceiveTimeout = value; }

        public string Host { get; set; }

        public int Port { get; set; }

    }
}
