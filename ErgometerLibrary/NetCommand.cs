using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ErgometerLibrary
{
    public class NetCommand
    {
        public enum CommandType { LOGIN, DATA, CHAT, LOGOUT, SESSION, VALUESET, USER, RESPONSE, REQUEST, LENGTH, SESSIONDATA, ERROR, BROADCAST }
        public enum RequestType { USERS, ALLSESSIONS, OLDDATA, SESSIONDATA, CHAT }
        public enum ResponseType { LOGINOK, LOGINWRONG, ERROR, NOTLOGGEDIN }
        public enum ValueType { TIME, POWER, ENERGY, DISTANCE }
        public enum LengthType { USERS, SESSIONS, SESSIONDATA, DATA }

        public double Timestamp { get; set; }
        public int Session { get; set; }
        public CommandType Type { get; set; }
        public ResponseType Response { get; set; }
        public ValueType Value { get; set; }
        public RequestType Request { get; set; }
        public LengthType Length { get; set; }
        public int SetValue { get; set; }
        public string DisplayName { get; set; }
        public bool IsDoctor { get; set; }
        public string Password { get; set; }
        public string ChatMessage { get; set; }
        public Meting Meting { get; set; }
        public int LengthValue { get; set; }

        //SESSION
        public NetCommand(int session)
        {
            Type = CommandType.SESSION;
            Session = session;
            Timestamp = Helper.Now;
        }

        //SESSIONDATA
        public NetCommand(string name, double timestamp, int session)
        {
            Type = CommandType.SESSIONDATA;
            Session = session;
            Timestamp = timestamp;
            DisplayName = name;
        }

        //RESPONSE
        public NetCommand(ResponseType response, int session)
        {
            Type = CommandType.RESPONSE;
            Session = session;
            Timestamp = Helper.Now;
            Response = response;
        }

        //Length
        public NetCommand(LengthType lengthtype, int length, int session)
        {
            Type = CommandType.LENGTH;
            Length = lengthtype;
            Session = session;
            Timestamp = Helper.Now;
            LengthValue = length;
        }

        //REQUEST
        public NetCommand(RequestType request, int session)
        {
            Type = CommandType.REQUEST;
            Session = session;
            Timestamp = Helper.Now;
            Request = request;
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
        public NetCommand(string chat, bool isDoctor, int session)
        {
            Type = CommandType.CHAT;
            Session = session;
            IsDoctor = isDoctor;
            Timestamp = Helper.Now;

            ChatMessage = chat.Replace("\n", "");
        }

        //BROADCAST
        public NetCommand(string broadcast, int session)
        {
            Type = CommandType.BROADCAST;
            Session = session;
            Timestamp = Helper.Now;

            ChatMessage = broadcast.Replace("\n", "");
        }

        //SETVALUE
        public NetCommand(ValueType value, int val, int session)
        {
            Type = CommandType.VALUESET;
            Session = session;
            Value = value;
            SetValue = val;
        }

        //USER
        public NetCommand(string username, string password, int session)
        {
            Type = CommandType.USER;
            Session = session;
            DisplayName = username;
            Password = password;
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

            int comType = 0;
            try
            {
                comType = int.Parse(com[0]);
            }
            catch(Exception e)
            {
                return new NetCommand(CommandType.ERROR, 0);
            }

            int session = 0;
            if (com[1].StartsWith("ses"))
                session = int.Parse(com[1].Substring(3));
            else
                throw new FormatException("Error in NetCommand: " + com[1] + " is not a valid session.");

            string[] args = new string[com.Length - 2];
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
                    return ParseUser(session, args);
                case 9:
                    return ParseRequest(session, args);
                case 10:
                    return ParseLength(session, args);
                case 11:
                    return ParseSessionData(session, args);
                case 12:
                    return ParseBroadcast(session, args);
                default:
                    throw new FormatException("Error in NetCommand: " + comType + " is not a valid command type.");
            }
        }

        private static NetCommand ParseBroadcast(int session, string[] args)
        {
            if (args.Length != 1)
                throw new MissingFieldException("Error in NetCommand: Broadcast Message is missing arguments");

            NetCommand temp = new NetCommand(args[0], session);

            return temp;
        }

        private static NetCommand ParseSessionData(int session, string[] args)
        {
            if (args.Length != 2)
                throw new MissingFieldException("Error in NetCommand: Session Data is missing arguments");

            NetCommand temp = new NetCommand(args[0], double.Parse(args[1]), session);

            return temp;
        }

        private static NetCommand ParseLength(int session, string[] args)
        {
            if (args.Length != 2)
                throw new MissingFieldException("Error in NetCommand: Length is missing arguments");

            switch (args[0])
            {
                case "users":
                    return new NetCommand(LengthType.USERS, int.Parse(args[1]), session);
                case "sessiondata":
                    return new NetCommand(LengthType.SESSIONDATA, int.Parse(args[1]), session);
                case "sessions":
                    return new NetCommand(LengthType.SESSIONS, int.Parse(args[1]), session);
                case "data":
                    return new NetCommand(LengthType.DATA, int.Parse(args[1]), session);
                default:
                    throw new FormatException("Error in NetCommand: Length type not recognised");
            }
        }

        private static NetCommand ParseRequest(int session, string[] args)
        {
            if (args.Length != 1)
                throw new MissingFieldException("Error in NetCommand: Request is missing arguments");

            switch (args[0])
            {
                case "users":
                    return new NetCommand(RequestType.USERS, session);
                case "allsessions":
                    return new NetCommand(RequestType.ALLSESSIONS, session);
                case "olddata":
                    return new NetCommand(RequestType.OLDDATA, session);
                case "sessiondata":
                    return new NetCommand(RequestType.SESSIONDATA, session);
                case "chat":
                    return new NetCommand(RequestType.CHAT, session);
                default:
                    throw new FormatException("Error in NetCommand: Request type not recognised");
            }
        }

        private static NetCommand ParseUser(int session, string[] args)
        {
            if (args.Length != 2)
                throw new MissingFieldException("Error in NetCommand: User is missing arguments");

            NetCommand temp = new NetCommand(args[0], Helper.Base64Decode(args[1]), session);

            return temp;
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

            switch (args[0])
            {
                case "loginok":
                    return new NetCommand(ResponseType.LOGINOK, session);
                case "loginwrong":
                    return new NetCommand(ResponseType.LOGINWRONG, session);
                case "notloggedin":
                    return new NetCommand(ResponseType.NOTLOGGEDIN, session);
                case "error":
                    return new NetCommand(ResponseType.ERROR, session);
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
            if (args.Length != 2)
                throw new MissingFieldException("Error in NetCommand: Chat Message is missing arguments");

            NetCommand temp = new NetCommand(CommandType.CHAT, session);
            temp.ChatMessage = args[0];
            temp.IsDoctor = bool.Parse(args[1]);

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
            temp.Password = Helper.Base64Decode(args[1]);

            return temp;
        }

        public override string ToString()
        {
            string command = "";

            switch (Type)
            {
                case CommandType.LOGIN:
                    command += "1»ses" + Session + "»" + DisplayName + "»" + Helper.Base64Encode(Password) + "»" + IsDoctor;
                    break;
                case CommandType.DATA:
                    command += "2»ses" + Session + "»" + Meting.ToCommand();
                    break;
                case CommandType.CHAT:
                    command += "3»ses" + Session + "»" + ChatMessage + "»" + IsDoctor;
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
                    command += "8»ses" + Session + "»" + DisplayName + "»" + Helper.Base64Encode(Password);
                    break;
                case CommandType.REQUEST:
                    command += "9»ses" + Session + "»" + Request.ToString().ToLower();
                    break;
                case CommandType.LENGTH:
                    command += "10»ses" + Session + "»" + Length.ToString().ToLower() + "»" + LengthValue;
                    break;
                case CommandType.SESSIONDATA:
                    command += "11»ses" + Session + "»" + DisplayName + "»" + Timestamp;
                    break;
                case CommandType.BROADCAST:
                    command += "12»ses" + Session + "»" + ChatMessage;
                    break;
                case CommandType.ERROR:
                    command += "ERROR IN NETCOMMAND";
                    break;

                default:
                    throw new FormatException("Error in NetCommand: Cannot find type of command");
            }

            return command;
        }
    }
}
