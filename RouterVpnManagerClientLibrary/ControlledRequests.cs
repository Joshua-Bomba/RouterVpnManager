using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RouterVpnManagerClientLibrary
{
    public class ControlledRequests
    {
        public ControlledRequests()
        {
            
        }

        public static JObject FormatMessage(string type,string request, object data = null)
        {
            JObject obj = new JObject();
            obj["type"] = type;
            obj["request"] = request;
            obj["data"] = data != null ? JObject.FromObject(data) : new JObject();
            return obj;
        }
    }
}
