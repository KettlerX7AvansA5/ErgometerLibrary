using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace ErgometerLibrary
{
    public class NetHelper
    {
        public static void SendNetCommand(TcpClient client, NetCommand command)
        {
            /*
            byte[] b = Encoding.Unicode.GetBytes(command.ToString());
            client.GetStream().Write(b, 0, b.Length);
            client.GetStream().Flush();
            */
            SendString(client, command.ToString());
        }

        public static void SendString(TcpClient client, string command)
        {
            /*
            byte[] b = Encoding.Unicode.GetBytes(command);
            client.GetStream().Write(b, 0, b.Length);
            client.GetStream().Flush();
            */

            if (client.Connected)
            {
                try {
                    StreamWriter wr = new StreamWriter(client.GetStream(), Encoding.Unicode);
                    wr.WriteLine(command);
                    wr.Flush();
                }
                catch(Exception e)
                {
                    return;
                }
            }
        }

        public static NetCommand ReadNetCommand(TcpClient client)
        {
            /*
            byte[] bytesFrom = new byte[(int) client.ReceiveBufferSize];
            client.GetStream().Read(bytesFrom, 0, (int)client.ReceiveBufferSize);
            string response = Encoding.Unicode.GetString(bytesFrom);
            NetCommand net = NetCommand.Parse(response);
            return net;
            */

            string str = ReadString(client);
            NetCommand net;
            if (str != "")
                try {
                    net = NetCommand.Parse(str);
                }
                catch(Exception e)
                {
                    net = new NetCommand(NetCommand.CommandType.ERROR, 0);
                }
            else
                net = new NetCommand(NetCommand.CommandType.ERROR, 0);
            return net;
        }

        public static string ReadString(TcpClient client)
        {
            /*
            byte[] bytesFrom = new byte[(int)client.ReceiveBufferSize];
            client.GetStream().Read(bytesFrom, 0, (int)client.ReceiveBufferSize);
            string response = Encoding.Unicode.GetString(bytesFrom);
            return response;
            */
            if (client.Connected)
            {
                try {
                    StreamReader rd = new StreamReader(client.GetStream(), Encoding.Unicode);
                    string str = rd.ReadLine();
                    return str;
                }
                catch(Exception e)
                {
                    return "";
                }
            }
            else
                return "";
        }

        public static IPAddress GetIP(string ipstring)
        {
            IPAddress ip;

            bool ipIsOk = IPAddress.TryParse(ipstring, out ip);
            if (!ipIsOk) { return null; }

            return ip;
        }

        public static IPAddress GetIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return GetIP(localIP);
        }
    }
}
