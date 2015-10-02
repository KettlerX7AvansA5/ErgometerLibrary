using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErgometerLibrary
{
    public class NetCommand
    {
        public enum CommandType { LOGIN, DATA, CHAT, LOGOUT, SESSION };


        public double Timestamp { get; set; }
        public int Session { get; set; }
        public CommandType Type { get; set; }
        public string DisplayName { get; set; }
        public bool IsDoctor { get; set; }
        public string Password { get; set; }
        public string ChatMessage { get; set; }
        public Meting Meting { get; set; }

        public NetCommand(int session)
        {
            Type = CommandType.SESSION;
            Session = session;
            Timestamp = Helper.Now;
        }

        public NetCommand(CommandType commandtype, int session)
        {
            Type = commandtype;
            Session = session;
            Timestamp = Helper.Now;
        }

        public NetCommand(Meting m, int session)
        {
            Type = CommandType.DATA;
            Session = session;
            Timestamp = Helper.Now;

            Meting = m;
        }

        public NetCommand(string chat, int session)
        {
            Type = CommandType.CHAT;
            Session = session;
            Timestamp = Helper.Now;

            ChatMessage = chat;
        }

        public NetCommand(string name, bool doctor, int session)
        {
            Type = CommandType.LOGIN;
            Session = session;
            Timestamp = Helper.Now;

            DisplayName = name;
            IsDoctor = doctor;
        }

        public static NetCommand Parse(string command)
        {
            string[] com = command.Split('»');

            int comType = int.Parse(com[0]);
            int session = 0;
            if (com[1].StartsWith("ses"))
                session = int.Parse(com[1].Substring(3));
            else
                throw new FormatException("Error in NetCommend: " + com[1] + " is not a valid session.");

            string[] args = new string[com.Length-2];
            for (int i = 2; i < com.Length; i++)
            {
                args[i - 2] = com[i];
            }

            switch (comType)
            {
                case 1:
                    return ParseLoginRequest(session, args);
                case 2:
                    return ParseData(session, args);
                case 3:
                    return ParseChatMessage(session, args);
                case 4:
                    return ParseLogoutRequest(session, args);
                case 5:
                    return ParseSession(session);
                default:
                    throw new FormatException("Error in NetCommand: " + comType + " is not a valid command type.");
            }
        }

        private static NetCommand ParseSession(int session)
        {
            NetCommand temp = new NetCommand(CommandType.SESSION, session);
            return temp;
        }

        private static NetCommand ParseLogoutRequest(int session, string[] args)
        {
            if (args.Length != 1)
                throw new MissingFieldException("Error in NetCommand: Logout Request is missing arguments");

            NetCommand temp = new NetCommand(CommandType.LOGOUT, session);
            if (args[0] != "logout")
                throw new FormatException("Error in NetCommand: " + args[0] + " is not a valid logout request");

            return temp;
        }

        private static NetCommand ParseChatMessage(int session, string[] args)
        {
            if (args.Length != 1)
                throw new MissingFieldException("Error in NetCommand: Chat Message is missing arguments");

            NetCommand temp = new NetCommand(CommandType.CHAT, session);
            temp.ChatMessage = args[0];

            return temp;
        }

        private static NetCommand ParseData(int session, string[] args)
        {
            if (args.Length != 9)
                throw new MissingFieldException("Error in NetCommand: Data is missing arguments");

            NetCommand temp = new NetCommand(CommandType.DATA, session);
            temp.Meting = Meting.Parse(string.Join("\t", args));

            return temp;
        }

        private static NetCommand ParseLoginRequest(int session, string[] args)
        {
            bool doctor = bool.Parse(args[1]);
            if (args.Length != 3)
                throw new MissingFieldException("Error in NetCommand: Doctor login is missing arguments");

            NetCommand temp = new NetCommand(CommandType.LOGIN, session);
            temp.IsDoctor = doctor;
            temp.DisplayName = args[0];
            temp.Password = args[2];

            return temp;
        }

        public override string ToString()
        {
            string command = "";

            switch (Type)
            {
                case CommandType.LOGIN:
                    command += "1»ses" + Session + "»" + DisplayName + "»" + IsDoctor +  "»" + Password;
                    break;
                case CommandType.DATA:
                    command += "2»ses" + Session + "»" + Meting.ToCommand();
                    break;
                case CommandType.CHAT:
                    command += "3»ses" + Session + "»" + ChatMessage;
                    break;
                case CommandType.LOGOUT:
                    command += "4»ses" + Session + "»logout";
                    break;
                case CommandType.SESSION:
                    command += "5»ses" + Session;
                    break;

                default:
                    throw new FormatException("Error in NetCommand: Cannot find type of command");
            }

            return command;
        }
    }
}
