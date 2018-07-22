using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RouterVpnManagerClientLibrary.ServerResponseObjects;

namespace RouterVpnManagerClientLibrary
{
    public interface IBroadcastListener
    {
        void ConnectToVpn(ConnectToVpnResponse response);

        void DisconnectFromVpn(DisconnectFromVpnResponse response);

        void BroadcastRecieved(BroadcastMessage message);
    }
}
