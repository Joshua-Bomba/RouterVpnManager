using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterVpnManagerClientLibrary.ServerResponseObjects
{
    public class StatusResponse : ResponseBase
    {
        public bool Status { get; set; }

        public string ExceptionMessage { get; set; }

        public override void SetData()
        {
            Status = string.IsNullOrWhiteSpace(Data["status"].ToString());
            ExceptionMessage = Data["status"].ToString();
        }

        public static implicit operator bool(StatusResponse response)
        {
            return !object.ReferenceEquals(response, null) && response.Status;
        }

        public static implicit operator StatusResponse(bool state)
        {
            return new StatusResponse {Status = state, ExceptionMessage = ""};
        }
    }
}
