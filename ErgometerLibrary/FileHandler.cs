using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ErgometerLibrary
{
    public class FileHandler
    {
        public static string DataFolder { get; } = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Ergometer");

        public static void CheckDataFolder()
        {
            if(! Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }
        }

        public static int GenerateSession()
        {
            string[] existingSessions = Directory.GetDirectories(DataFolder);

            Random rand = new Random();
            int sessionID = rand.Next(0, int.MaxValue);


            while (existingSessions.Contains(sessionID.ToString()))
            {
                sessionID = rand.Next(int.MinValue, int.MaxValue);
            }

            return sessionID;
        }

        public static void CreateSession(int session, string naam)
        {
            Directory.CreateDirectory(GetSessionFolder(session));
            File.Create(Path.Combine(GetSessionFolder(session), "session.prop"));
            File.Create(Path.Combine(GetSessionFolder(session), "metingen.ergo"));
            File.Create(Path.Combine(GetSessionFolder(session), "chat.log"));

            File.WriteAllText(GetSessionFile(session), naam + "\n" + Helper.Now);
        }

        public static void WriteMetingen(int session, List<Meting> metingen)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(metingen);
            File.WriteAllText(GetSessionMetingen(session), json);
            Console.WriteLine("Writing metingen: " + GetSessionMetingen(session));
        }

        public static void WriteChat(int session, List<ChatMessage> chat)
        {
            string write = "";
            foreach(ChatMessage c in chat)
            {
                write += c.ToString() + "\n";
            }

            File.WriteAllText(GetSessionChat(session), write);
            Console.WriteLine("Writing chat: " + GetSessionChat(session));
        }

        private static string GetSessionFolder(int session)
        {
            return Path.Combine(DataFolder, session.ToString());
        }
        private static string GetSessionFile(int session)
        {
            return Path.Combine(DataFolder, session.ToString(), "session.prop");
        }
        private static string GetSessionMetingen(int session)
        {
            return Path.Combine(DataFolder, session.ToString(), "metingen.ergo");
        }
        private static string GetSessionChat(int session)
        {
            return Path.Combine(DataFolder, session.ToString(), "chat.log");
        }
    }
}
