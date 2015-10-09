using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErgometerLibrary
{
    public class NetCommand
    {
        public enum CommandType { LOGIN, DATA, CHAT, LOGOUT, SESSION, VALUESET, USER, RESPONSE, REQUEST };
        public enum RequestType { USERS}
        public enum ResponseType { LOGINOK, LOGINWRONG, ERROR, NOTLOGGEDIN }
        public enum ValueType { TIME, POWER, ENERGY, DISTANCE }

        public double Timestamp { get; set; }
        public int Session { get; set; }
        public CommandType Type { get; set; }
        public ResponseType Response { get; set; }
        public ValueType Value { get; set; }
        public RequestType Request { get; set; }
        public double SetValue { get; set; }
        public string DisplayName { get; set; }
        public bool IsDoctor { get; set; }
        public string Password { get; set; }
        public string ChatMessage { get; set; }
        public Meting Meting { get; set; }
        public Dictionary<string, string> users;
        
        //SESSION
        public NetCommand(int session)
        {
            Type = CommandType.SESSION;
            Session = session;
            Timestamp = Helper.Now;
        }

        //RESPONSE
        public NetCommand(ResponseType response, int session)
        {
            Type = CommandType.RESPONSE;
            Session = session;
            Timestamp = Helper.Now;
            Response = response;
        }

        //STANDARD
        public NetCommand(CommandType commandtype, int session)
        {
            Type = commandtype;
            Session = session;
            Timestamp = Helper.Now;
        }

        //METING
        public NetCommand(Meting m, int session)
        {
            Type = CommandType.DATA;
            Session = session;
            Timestamp = Helper.Now;

            Meting = m;
        }

        //CHAT
        public NetCommand(string chat, int session)
        {
            Type = CommandType.CHAT;
            Session = session;
            Timestamp = Helper.Now;

            ChatMessage = chat;
        }

        //SETVALUE
        public NetCommand(ValueType value, int val, int session)
        {
            Type = CommandType.VALUESET;
            Session = session;
            Value = value;
            SetValue = val;
        }

        //USERS
        public NetCommand(Dictionary<string, string> users, int session)
        {
            Type = CommandType.USER;
            Session = session;
            this.users = users;
        }

        //LOGIN
        public NetCommand(string name, bool doctor, string password, int session)
        {
            Type = CommandType.LOGIN;
            Session = session;
            Timestamp = Helper.Now;

            DisplayName = name;
            IsDoctor = doctor;
            Password = password;
        }

        public static NetCommand Parse(string command)
        {
            string[] com = command.Split('»');

            Console.WriteLine(command);

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
                case 6:
                    return ParseResponse(session, args);
                case 7:
                    return ParseValue(session, args);
                case 8:
                    return ParseUsers(session, args);
                default:
                    throw new FormatException("Error in NetCommand: " + comType + " is not a valid command type.");
            }
        }

        private static NetCommand ParseUsers(int session, string[] args)
        {
            if (args.Length < 2)
                throw new MissingFieldException("Error in NetCommand: Users is missing arguments");

            Dictionary<string, string> users = new Dictionary<string, string>();

            foreach(string str in args)
            {
                string[] temp = str.Split('|');
                string name = temp[0];
                string password = temp[1];
                users.Add(name, password);
            }

            return new NetCommand(users, session);
        }

        private static NetCommand ParseValue(int session, string[] args)
        {
            if (args.Length != 2)
                throw new MissingFieldException("Error in NetCommand: SetValue is missing arguments");

            switch (args[0])
            {
                case "time":
                    return new NetCommand(ValueType.TIME, int.Parse(args[1]), session);
                case "power":
                    return new NetCommand(ValueType.POWER, int.Parse(args[1]), session);
                case "energy":
                    return new NetCommand(ValueType.ENERGY, int.Parse(args[1]), session);
                case "distance":
                    return new NetCommand(ValueType.DISTANCE, int.Parse(args[1]), session);
                default:
                    throw new FormatException("Error in NetCommand: SetValue type not recognised");
            }
        }

        private static NetCommand ParseResponse(int session, string[] args)
        {
            if (args.Length != 1)
                throw new MissingFieldException("Error in NetCommand: Response is missing arguments");

            switch(args[0])
            {
                case "loginok":
                    return new NetCommand(ResponseType.LOGINOK, session);
                case "loginwrong":
                    return new NetCommand(ResponseType.LOGINWRONG, session);
                case "notloggedin":
                    return new NetCommand(ResponseType.NOTLOGGEDIN, session);
                default:
                    throw new FormatException("Error in NetCommand: Response type not recognised");
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
            bool doctor = bool.Parse(args[2]);
            if (args.Length != 3)
                throw new MissingFieldException("Error in NetCommand: Doctor login is missing arguments");

            NetCommand temp = new NetCommand(CommandType.LOGIN, session);
            temp.IsDoctor = doctor;
            temp.DisplayName = args[0];
            temp.Password = args[1];

            return temp;
        }

        public override string ToString()
        {
            string command = "";

            switch (Type)
            {
                case CommandType.LOGIN:
                    command += "1»ses" + Session + "»" + DisplayName +  "»" + Password + "»" + IsDoctor;
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
                case CommandType.RESPONSE:
                    command += "6»ses" + Session + "»" + Response.ToString().ToLower();
                    break;
                case CommandType.VALUESET:
                    command += "7»ses" + Session + "»" + Value.ToString().ToLower() + "»" + SetValue;
                    break;
                case CommandType.USER:
                    command += "8»ses" + Session;
                    foreach(KeyValuePair<string, string> keys in users)
                    {
                        command += "»" + keys.Key + "|" + keys.Value;
                    }
                    break;

                default:
                    throw new FormatException("Error in NetCommand: Cannot find type of command");
            }

            return command;
        }
    }
}
