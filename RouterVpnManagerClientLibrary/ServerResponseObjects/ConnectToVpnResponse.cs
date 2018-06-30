using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RouterVpnManagerClientLibrary.ServerResponseObjects
{
    [JsonObject]
    public class ConnectToVpnResponse :ResponseBase
    {
        public string VpnLocation { get; set; }

        public string Status { get; set; }
        public override void SetData()
        {
            VpnLocation = Data["vpnLocation"].ToObject<string>();
            Status = Data["status"].ToObject<string>();
        }
    }
}
