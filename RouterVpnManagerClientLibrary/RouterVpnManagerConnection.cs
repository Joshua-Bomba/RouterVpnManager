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
        private BlockingCollection<KeyValuePair<string,Callback>> requests_;

        public delegate bool Callback(JObject message);

        public RouterVpnManagerConnection()
        {
            client_ = new TcpClient();
            callbacks_ = new ConcurrentDictionary<string, Callback>();
            requests_ = new BlockingCollection<KeyValuePair<string,Callback>>();
            SetDefaults();
        }

        public bool Connect()
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Host), Port);
                client_.Connect(ep);

                ProcessResponses();

                JObject obj = ControlledRequests.FormatMessage("request","connection");
                byte[] bytes = Encoding.ASCII.GetBytes(obj.ToString());

                NetworkStream ns = client_.GetStream();
                ns.Write(bytes, 0, bytes.Length);

                AddCallback("connection", (JObject message) =>
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


        private void AddCallback(string response,Callback callback)
        {
            ManualResetEvent oSignalEvent = new ManualResetEvent(false);
            try
            {
                KeyValuePair<string, Callback> waitFor = new KeyValuePair<string, Callback>(response, (JObject obj) =>
                {
                    bool state = callback(obj);
                    oSignalEvent.Set();
                    return state;
                });
                requests_.Add(waitFor);
                oSignalEvent.WaitOne(Properties.Settings.Default.Timeout);
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


        public void Disconnect()
        {
            client_.Close();
        }

        public bool SendJson(JObject obj, Callback callback = null)
        {
            if (client_.Connected)
            {
                NetworkStream ns = client_.GetStream();
                byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
                ns.Write(bytes, 0, bytes.Length);
                if (callback != null)
                {
                    AddCallback(obj["request"].ToString(),callback);
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

        protected void ProcessResponses()
        {
            Task.Run(() =>
            {
                for (;;)
                {
                    try
                    {
                        NetworkStream ns = client_.GetStream();
                        byte[] bytesResponse = new byte[client_.ReceiveBufferSize];
                        int bytesRead = ns.Read(bytesResponse, 0, client_.ReceiveBufferSize);
                        string dataReceived = Encoding.ASCII.GetString(bytesResponse, 0, bytesRead);
                        JObject obj = JObject.Parse(dataReceived);
                        if (obj["type"].ToString() == "broadcast")
                        {
                            if (callbacks_.ContainsKey(obj["request"].ToString()))
                            {
                                Callback callback;
                                callbacks_.TryGetValue(obj["request"].ToString(), out callback);
                                callback?.Invoke(obj);
                            }
                        }
                        else if (obj["type"].ToString() == "response")
                        {
                            KeyValuePair<string,Callback> response = requests_.FirstOrDefault(x => x.Key == obj["request"].ToString());
                            response.Value?.Invoke(obj);
                        }
                    }
                    catch (Exception e)
                    {
                        //This will occure every 5 seconds because of the timeout
                        //This will also allow time to check if the program is still running
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
