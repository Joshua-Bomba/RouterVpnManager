using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RouterVpnManagerClientLibrary
{
    public class RequestProcessor
    {

        public delegate void Callback(JObject message);


        private TcpClient client_;
        private bool responseProcessListener_;
        private bool responseInjestionListener_;
        private Task responseInjestionTask_;
        private Task responseProcessTask_;
        private BlockingCollection<JObject> responses_;
        private ConcurrentDictionary<string, Callback> broadcastCallbacks_;
        private ConcurrentDictionary<JObject, Callback> privateCallbacks_;

        public RequestProcessor(TcpClient client)
        {
            this.client_ = client;
            responseInjestionListener_ = true;
            responseProcessListener_ = true;
            responseInjestionTask_ = null;
            responseProcessTask_ = null;
            responses_ = new BlockingCollection<JObject>();
            broadcastCallbacks_ = new ConcurrentDictionary<string, Callback>();
            privateCallbacks_ = new ConcurrentDictionary<JObject, Callback>();
        }


        public void Start()
        {
            ProcessInjestion();
            ProcessResponses();
        }

        public async void Stop()
        {
            responseProcessListener_ = false;
            responseInjestionListener_ = false;

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



        public bool AddPublicCallbackHandler(string request, RequestProcessor.Callback callback)
        {
            if (!this.broadcastCallbacks_.ContainsKey(request))
            {
                this.broadcastCallbacks_.TryAdd(request, callback);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        /// <param name="oSignalEvent">Will be set asoon as the callback is called</param>
        /// <returns></returns>
        public bool AddPrivateCallbackHandler(JObject obj, Callback callback)
        {
            if (!this.privateCallbacks_.ContainsKey(obj))
            {
                return privateCallbacks_.TryAdd(obj, callback);
            }
            return false;
        }



        protected void ProcessInjestion()
        {
            responseInjestionTask_ = Task.Run(() =>
            {
                while (responseInjestionListener_)
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
                            if (broadcastCallbacks_.ContainsKey(obj["request"].ToString()))
                            {
                                broadcastCallbacks_.TryGetValue(obj["request"].ToString(), out var callback);
                                callback?.Invoke(obj);
                            }
                        }
                        else if (obj["type"].ToString() == "response")
                        {
                            Callback response;
                            JObject key = privateCallbacks_.FirstOrDefault(x => x.Key["request"].ToString() == obj["request"].ToString() && x.Key["signature"].ToString() == obj["signature"].ToString()).Key;
                            if (key != null)
                            {
                                if (privateCallbacks_.TryRemove(key, out response))
                                {
                                    response?.Invoke(obj);
                                }
                                else
                                {
                                    RouterVpnManagerLogLibrary.Log("Error removing ");
                                }

                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        RouterVpnManagerLogLibrary.Log("Something Blew Up: " + ex.ToString());
                    }
                }
            });
        }
    }
}
