using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErgometerLibrary
{
    public class NetCommand
    {
        public enum CommandType { LOGIN, DATA, CHAT, LOGOUT };


        public double Timestamp { get; set; }
        public int Session { get; set; }
        public CommandType Type { get; set; }
        public string DisplayName { get; set; }
        public bool IsDoctor { get; set; }
        public string Password { get; set; }
        public string ChatMessage { get; set; }
        public Meting Meting { get; set; }


        public NetCommand(CommandType commandtype, int session)
        {
            Type = commandtype;
            Session = session;
            Timestamp = (DateTime.Now - DateTime.Parse("1/1/1870 0:0:0")).TotalMilliseconds;
        }

        public NetCommand(Meting m, int session)
        {
            Type = CommandType.DATA;
            Session = session;
            Timestamp = (DateTime.Now - DateTime.Parse("1/1/1870 0:0:0")).TotalMilliseconds;

            Meting = m;
        }

        public NetCommand(string chat, int session)
        {
            Type = CommandType.CHAT;
            Session = session;
            Timestamp = (DateTime.Now - DateTime.Parse("1/1/1870 0:0:0")).TotalMilliseconds;

            ChatMessage = chat;
        }

        public NetCommand(string name, bool doctor, int session)
        {
            Type = CommandType.LOGIN;
            Session = session;
            Timestamp = (DateTime.Now - DateTime.Parse("1/1/1870 0:0:0")).TotalMilliseconds;

            DisplayName = name;
            IsDoctor = doctor;
        }

        public NetCommand Parse(string command)
        {
            string[] com = command.Split('»');

            int comType = int.Parse(com[0]);
            int session = 0;
            if (com[1].StartsWith("ses"))
                session = int.Parse(com[1].Substring(3));
            else
                throw new FormatException("Error in NetCommend: " + com[1] + " is not a valid session.");

            string[] args = (string[]) com.Skip(2);

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
                default:
                    throw new FormatException("Error in NetCommand: " + comType + " is not a valid command type.");
            }
        }

        private NetCommand ParseLogoutRequest(int session, string[] args)
        {
            if (args.Length != 1)
                throw new MissingFieldException("Error in NetCommand: Logout Request is missing arguments");

            NetCommand temp = new NetCommand(CommandType.LOGOUT, session);
            if (args[0] != "logout")
                throw new FormatException("Error in NetCommand: " + args[0] + " is not a valid logout request");

            return temp;
        }

        private NetCommand ParseChatMessage(int session, string[] args)
        {
            if (args.Length != 1)
                throw new MissingFieldException("Error in NetCommand: Chat Message is missing arguments");

            NetCommand temp = new NetCommand(CommandType.CHAT, session);
            temp.ChatMessage = args[0];

            return temp;
        }

        private NetCommand ParseData(int session, string[] args)
        {
            if (args.Length != 9)
                throw new MissingFieldException("Error in NetCommand: Data is missing arguments");

            NetCommand temp = new NetCommand(CommandType.DATA, session);
            temp.Meting = Meting.Parse(string.Join("\t", args));

            return temp;
        }

        private NetCommand ParseLoginRequest(int session, string[] args)
        {
            bool doctor = bool.Parse(args[1]);
            if (doctor && args.Length != 5)
                throw new MissingFieldException("Error in NetCommand: Doctor login is missing arguments");
            else if(args.Length != 4)
                throw new MissingFieldException("Error in NetCommand: Client login is missing arguments");

            NetCommand temp = new NetCommand(CommandType.LOGIN, session);
            temp.IsDoctor = doctor;
            temp.DisplayName = args[0];
            if (doctor)
                temp.Password = args[2];

            return temp;
        }

        public override string ToString()
        {
            string command = "";

            switch(Type)
            {
                case CommandType.LOGIN:
                    command += "1»ses" + Session + "»" + DisplayName + "»" + IsDoctor + (IsDoctor ? "»" + Password : "");
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

                default:
                    throw new FormatException("Error in NetCommand: Cannot find type of command");
            }

            return command;
        }
    }
}
