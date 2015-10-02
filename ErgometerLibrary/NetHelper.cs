using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ErgometerLibrary
{
    class NetHelper
    {
        public static void SendNetCommand(TcpClient client, NetCommand command)
        {
            byte[] b = Encoding.ASCII.GetBytes(command.ToString());
            client.GetStream().Write(b, 0, b.Length);
            client.GetStream().Flush();
        }

        public static NetCommand ReadNetCommand(TcpClient client)
        {
            byte[] bytesFrom = new byte[(int) client.ReceiveBufferSize];
            client.GetStream().Read(bytesFrom, 0, (int)client.ReceiveBufferSize);
            String response = Encoding.ASCII.GetString(bytesFrom);
            NetCommand net = NetCommand.Parse(response);
            return net;
        }

        public static IPAddress GetIP(string ipstring)
        {
            IPAddress ip;

            bool ipIsOk = IPAddress.TryParse(ipstring, out ip);
            if (!ipIsOk) { return null; }

            return ip;
        }
    }
}
