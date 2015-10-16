﻿using System;
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
                StreamWriter wr = new StreamWriter(client.GetStream(), Encoding.Unicode);
                wr.WriteLine(command);
                Console.WriteLine("sent " + command);
                wr.Flush();
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
            NetCommand net = NetCommand.Parse(str);
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
                StreamReader rd = new StreamReader(client.GetStream(), Encoding.Unicode);
                string str = rd.ReadLine();
                Console.WriteLine("rec " + str);
                return str;
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
    }
}
