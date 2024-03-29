﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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
        private ConcurrentDictionary<string, Callback> broadcastCallbacksHandlers_;
        private ConcurrentDictionary<JObject, Callback> privateCallbacks_;
        private ConcurrentDictionary<JObject, HasCallbackBeenRecieved> broadcastCallback_;

        public RequestProcessor(TcpClient client)
        {
            this.client_ = client;
            responseInjestionListener_ = true;
            responseProcessListener_ = true;
            responseInjestionTask_ = null;
            responseProcessTask_ = null;
            responses_ = new BlockingCollection<JObject>();
            broadcastCallbacksHandlers_ = new ConcurrentDictionary<string, Callback>();
            privateCallbacks_ = new ConcurrentDictionary<JObject, Callback>();
            broadcastCallback_ = new ConcurrentDictionary<JObject, HasCallbackBeenRecieved>();
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
            if (!this.broadcastCallbacksHandlers_.ContainsKey(request))
            {
                this.broadcastCallbacksHandlers_.TryAdd(request, callback);
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

        public bool AddBroadcastHandleListener(JObject obj, HasCallbackBeenRecieved callback)
        {
            if (!this.broadcastCallback_.ContainsKey(obj))
            {
                return broadcastCallback_.TryAdd(obj, callback);
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
                            if (broadcastCallbacksHandlers_.ContainsKey(obj["request"].ToString()))
                            {
                                broadcastCallbacksHandlers_.TryGetValue(obj["request"].ToString(), out var callback);
                                callback?.Invoke(obj);
                                Func<KeyValuePair<JObject, HasCallbackBeenRecieved>, bool> equalCheck = (KeyValuePair<JObject, HasCallbackBeenRecieved> x) =>
                                    x.Key["request"] != null && obj["request"] != null && x.Key["signature"] != null && obj["signature"] != null
                                    && x.Key["request"].ToString() == obj["request"].ToString() && x.Key["signature"].ToString() == obj["signature"].ToString();

                                if (broadcastCallback_.Any(equalCheck))//this may not work properly
                                {
                                    JObject key = broadcastCallback_.First(equalCheck).Key;
                                    HasCallbackBeenRecieved response;
                                    if (broadcastCallback_.TryRemove(key, out response))
                                    {
                                        response.SignalEvent.Set();
                                    }
                                    else
                                    {
                                        RouterVpnManagerLogLibrary.Log("Error removing ");
                                    }
                                }
                            }
                        }
                        else if (obj["type"].ToString() == "response")
                        {
                            Func<KeyValuePair<JObject, Callback>, bool> equalCheck = (KeyValuePair<JObject, Callback> x) => x.Key["request"].ToString() == obj["request"].ToString() && x.Key["signature"].ToString() == obj["signature"].ToString();
                            if (privateCallbacks_.Any(equalCheck))
                            {
                                JObject key = privateCallbacks_.Where(equalCheck).First().Key;
                                Callback response;
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
