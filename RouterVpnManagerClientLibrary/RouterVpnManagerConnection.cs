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
        private TcpClient client_;
        private ConcurrentDictionary<string, Callback> callbacks_;
        private ConcurrentBag<KeyValuePair<JObject,Callback>> requests_;
        private BlockingCollection<JObject> responses_;
        private bool responseInjestionListener_;
        private Task responseInjestionTask_;
        private bool responseProcessListener_;
        private Task responseProcessTask_;

        public delegate bool Callback(JObject message);

        public RouterVpnManagerConnection()
        {
            client_ = new TcpClient();
            callbacks_ = new ConcurrentDictionary<string, Callback>();
            requests_ = new ConcurrentBag<KeyValuePair<JObject,Callback>>();
            responses_ = new BlockingCollection<JObject>();
            responseInjestionListener_ = true;
            responseProcessListener_ = true;
            responseInjestionTask_ = null;
            responseProcessTask_ = null;
            SetDefaults();
        }

        public bool Connect()
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Host), Port);
                client_.Connect(ep);

                ProcessInjestion();
                ProcessResponses();

                JObject obj = ControlledRequests.FormatMessage("request","connection");
                byte[] bytes = Encoding.ASCII.GetBytes(obj.ToString());

                NetworkStream ns = client_.GetStream();
                ns.Write(bytes, 0, bytes.Length);

                AddCallback(obj, (JObject message) =>
                {
                    RouterVpnManagerLogLibrary.Log(message["data"].ToString());
                    return true;
                });

                return true;
            }
            catch (Exception e)
            {
                RouterVpnManagerLogLibrary.Log(e.ToString());
                return false;
            }

        }


        private bool AddCallback(JObject response,Callback callback)
        {
            ManualResetEvent oSignalEvent = new ManualResetEvent(false);
            try
            {
                KeyValuePair<JObject, Callback> waitFor = new KeyValuePair<JObject, Callback>(response, (JObject obj) =>
                {
                    bool state = callback(obj);
                    oSignalEvent.Set();
                    return state;
                });
                requests_.Add(waitFor);
                bool responseState = oSignalEvent.WaitOne(Properties.Settings.Default.Timeout);
                if (!responseState)
                {

                }

                return responseState;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                oSignalEvent.Reset();
            }

        }


        public async void Disconnect()
        {
            responseProcessListener_ = false;
            responseInjestionListener_ = false;
            client_.Close();

            //TODO: make this less shit
            if (responseInjestionTask_ != null)
            {
                await responseInjestionTask_;//This may(probably will) cause a lockup
            }

            if (responseProcessTask_ != null)
            {
                await responseProcessTask_;//This most definetly may cause a lockup
            }
        }

        public bool SendJson(JObject obj, Callback callback = null)
        {
            if (client_.Connected)
            {
                obj["signature"] = Guid.NewGuid().ToString();

                NetworkStream ns = client_.GetStream();
                byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
                ns.Write(bytes, 0, bytes.Length);
                if (callback != null)
                {
                    AddCallback(obj,callback);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddCallbackHandler(string request,Callback callback)
        {
            if (!this.callbacks_.ContainsKey(request))
            {
                this.callbacks_.TryAdd(request, callback);
                return true;
            }

            return false;
        }

        protected void ProcessInjestion()
        {
            responseInjestionTask_ = Task.Run(() =>
            {
                while(responseInjestionListener_)
                {
                    try
                    {
                        NetworkStream ns = client_.GetStream();
                        byte[] bytesResponse = new byte[client_.ReceiveBufferSize];
                        int bytesRead = ns.Read(bytesResponse, 0, client_.ReceiveBufferSize);
                        string dataReceived = Encoding.ASCII.GetString(bytesResponse, 0, bytesRead);
                        JObject obj = JObject.Parse(dataReceived);
                        responses_.Add(obj);
                    }
                    catch
                    {
                        
                    }
                }
            });
        }

        protected void ProcessResponses()
        {
            responseProcessTask_ = Task.Run(() =>
            {
                while (responseProcessListener_)
                {
                    try
                    {
                        JObject obj = responses_.Take();
                        if (obj["type"].ToString() == "broadcast")
                        {
                            if (callbacks_.ContainsKey(obj["request"].ToString()))
                            {
                                callbacks_.TryGetValue(obj["request"].ToString(), out var callback);
                                callback?.Invoke(obj);
                            }
                        }
                        else if (obj["type"].ToString() == "response")
                        {
                            KeyValuePair<JObject, Callback> response = requests_.FirstOrDefault(x => x.Key["request"].ToString() == obj["request"].ToString()&& x.Key["signature"].ToString() == obj["signature"].ToString());
                            response.Value?.Invoke(obj);
                        }
                    }
                    catch
                    {

                    }
                }
            });
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
