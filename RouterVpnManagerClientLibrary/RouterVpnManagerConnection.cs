using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RouterVpnManagerClientLibrary
{
    public class RouterVpnManagerConnection : IDisposable
    {
        private TcpClient client_;

        public RouterVpnManagerConnection()
        {
            client_ = new TcpClient();
            SetDefaults();
        }

        public bool SendPlainTextMessage(string text)
        {
            byte[] a1 = Encoding.ASCII.GetBytes(text);

            throw new NotImplementedException();
        }

        public void SendJson(JObject obj)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void SetDefaults()
        {
            Timeout = Properties.Settings.Default.Timeout;
            Host = Properties.Settings.Default.Host;
            Port = Properties.Settings.Default.Port;
        }

        public int Timeout { get => client_.SendTimeout; set => client_.SendTimeout = value; }

        public string Host { get; set; }

        public uint Port { get; set; }

    }
}
