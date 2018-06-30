using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RouterVpnManagerClientLibrary.ServerResponseObjects
{
    public class ConnectionStatusResponse : ResponseBase
    {
        
        public bool Running { get; set; }

        public string ConnectedTo { get; set; }
        public override void SetData()
        {
            Running = (Data["running"].ToObject<bool?>() ?? false);
            ConnectedTo = Data["connectedTo"].ToObject<string>();
        }
    }
}
