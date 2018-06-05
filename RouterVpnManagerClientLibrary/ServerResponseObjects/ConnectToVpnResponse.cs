﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RouterVpnManagerClientLibrary.ServerResponseObjects
{
    [JsonObject]
    public class ConnectToVpnResponse
    {
        public string VpnLocation { get; set; }

        public string Status { get; set; }
    }
}