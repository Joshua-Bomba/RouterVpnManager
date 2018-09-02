

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
