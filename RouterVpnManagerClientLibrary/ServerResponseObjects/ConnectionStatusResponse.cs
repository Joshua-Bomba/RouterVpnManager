using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RouterVpnManagerClientLibrary.ServerResponseObjects
{
    [JsonObject]
    public class ConnectionStatusResponse
    {
        
        public bool Running { get; set; }

        public string ConnectedTo { get; set; }
    }
}
