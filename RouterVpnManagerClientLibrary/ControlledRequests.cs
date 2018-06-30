using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RouterVpnManagerClientLibrary.ServerResponseObjects;

namespace RouterVpnManagerClientLibrary
{
    public class ControlledRequests
    {
        private RouterVpnManagerConnection connection_;
        private IBroadcastListener listener_;
        public ControlledRequests(RouterVpnManagerConnection connection)
        {
            this.connection_ = connection;
            AddBroadcastListener();
        }

        public void AddBroadcastListener(IBroadcastListener listener)
        {
            this.listener_ = listener;
        }


        public static JObject FormatMessage(string type,string request, object data = null)
        {
            JObject obj = new JObject();
            obj["type"] = type;
            obj["request"] = request;
            obj["data"] = data != null ? JObject.FromObject(data) : new JObject();
            return obj;
        }

        private void AddBroadcastListener()
        {
            connection_.AddBroadcastCallbackHandler("connecttovpn", (JObject response) =>
            {
                //RouterVpnManagerLogLibrary.Log("Connection has been made to " + response["data"].ToString());
                ConnectToVpnResponse ctvr = response.ToObject<ConnectToVpnResponse>();
                ctvr.SetData();
                listener_?.ConnectToVpn(ctvr);
                return true;
            });

            connection_.AddBroadcastCallbackHandler("disconnectfrompvpn", (JObject response) =>
            {
                //RouterVpnManagerLogLibrary.Log("Disconnection has been made from " + response["data"].ToString());
                DisconnectFromVpnResponse dfvr = response.ToObject<DisconnectFromVpnResponse>();
                dfvr.SetData();
                listener_?.DisconnectFromVpn(dfvr);
                return true;
            });
        }


        public IEnumerable<string> ListAvaliableVpns()
        {
            JObject obj = FormatMessage("request", "listovpn", null);
            IEnumerable<string> array = null;
            connection_.SendJson(obj, ((JObject response) =>
            {
                array = response["data"].ToArray().Select(x => x.ToString());
                return true;
            })).Wait();
            return array;
        }

        public void ConnectToVpn(string vpn)
        {
            dynamic d = new ExpandoObject();
            d.vpn = vpn;
            JObject obj = FormatMessage("request", "connecttovpn", d);
            connection_.SendJson(obj).Wait();
        }

        public void DisconnectFromVpn()
        {
            JObject obj = FormatMessage("request", "disconnectfrompvpn");
            connection_.SendJson(obj).Wait();
        }

        public ConnectionStatusResponse CheckCurrentConnection()
        {
            JObject obj = FormatMessage("request", "checkconnectionstatus");
            ConnectionStatusResponse currentConnection = null;
            connection_.SendJson(obj, (JObject response) =>
            {
                try
                {
                    currentConnection = response.ToObject<ConnectionStatusResponse>();
                    currentConnection.SetData();
                }
                catch (Exception e)
                {
                    RouterVpnManagerLogLibrary.Log(e.ToString());
                    return false;
                }
                return true;
            }).Wait();
            return currentConnection;
        }
    }
}
