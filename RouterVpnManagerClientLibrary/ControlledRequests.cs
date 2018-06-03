using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RouterVpnManagerClientLibrary
{
    public class ControlledRequests
    {
        private RouterVpnManagerConnection connection_;
        public ControlledRequests(RouterVpnManagerConnection connection)
        {
            this.connection_ = connection;
        }

        public static JObject FormatMessage(string type,string request, object data = null)
        {
            JObject obj = new JObject();
            obj["type"] = type;
            obj["request"] = request;
            obj["data"] = data != null ? JObject.FromObject(data) : new JObject();
            return obj;
        }

        public void AddBroadcastListener()
        {
            connection_.AddCallbackHandler("connecttovpn", (JObject response) =>
            {
                RouterVpnManagerLogLibrary.Log("Connection has been made to " + response["data"].ToString());
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
            }));
            return array;
        }

        public void ConnectToVpn(string vpn)
        {
            dynamic d = new ExpandoObject();
            d.vpn = vpn;
            JObject obj = FormatMessage("request", "connecttovpn", d);
            connection_.SendJson(obj);
        }

        public string CheckCurrentConnection()
        {
            JObject obj = FormatMessage("request", "checkconnectionstatus");
            string currentConnection = null;
            connection_.SendJson(obj, (JObject response) =>
            {
                currentConnection = response["data"].ToString();
                return true;
            });
            return currentConnection;
        }
    }
}
