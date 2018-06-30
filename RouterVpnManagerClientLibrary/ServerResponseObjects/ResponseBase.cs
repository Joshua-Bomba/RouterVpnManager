using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RouterVpnManagerClientLibrary.ServerResponseObjects
{
    [JsonObject]
    public abstract class ResponseBase
    {
        public string Request { get; set; }

        public string Signature { get; set; }

        public string Type { get; set; }

        public JObject Data { get; set; }

        public abstract void SetData();
    }
}
