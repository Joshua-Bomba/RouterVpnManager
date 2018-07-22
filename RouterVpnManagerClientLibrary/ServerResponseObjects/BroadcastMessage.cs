using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterVpnManagerClientLibrary.ServerResponseObjects
{
    public class BroadcastMessage :ResponseBase
    {
        public string Message { get; set; }
        public override void SetData()
        {
            Message = Data["message"].ToObject<string>();
        }
    }
}
