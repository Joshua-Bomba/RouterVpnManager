
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
