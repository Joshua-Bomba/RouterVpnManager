using RouterVpnManagerClientLibrary.ServerResponseObjects;

namespace RouterVpnManagerClientLibrary
{
    public interface IBroadcastListener
    {
        void ConnectToVpn(ConnectToVpnResponse response);

        void DisconnectFromVpn(DisconnectFromVpnResponse response);
    }
}
