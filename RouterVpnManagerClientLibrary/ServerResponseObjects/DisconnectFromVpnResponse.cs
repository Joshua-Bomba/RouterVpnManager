using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RouterVpnManagerClientLibrary.ServerResponseObjects
{
    [JsonObject]
    public class DisconnectFromVpnResponse
    {
        public string Reason { get; set; }

        public string Status { get; set; }
    }
}
