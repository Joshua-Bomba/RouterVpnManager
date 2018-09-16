using RouterVpnManagerClientLibrary.ServerResponseObjects;

namespace RouterVpnManagerClientLibrary
{
    public interface IBroadcastListener
    {
        void ConnectedToVpn(ConnectToVpnResponse response);

        void DisconnectedFromVpn(DisconnectFromVpnResponse response);
    }
}
