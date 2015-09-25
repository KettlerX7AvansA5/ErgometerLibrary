using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int sessionID = rand.Next(int.MinValue, int.MaxValue);


            while (existingSessions.Contains(sessionID.ToString()))
            {
                sessionID = rand.Next(int.MinValue, int.MaxValue);
            }

            return sessionID;
        }

        public static void CreateSession(string naam, int session)
        {
            Directory.CreateDirectory(GetSessionFolder(session));
            File.Create(Path.Combine(GetSessionFolder(session), "session.prop"));
            File.Create(Path.Combine(GetSessionFolder(session), "metingen.ergo"));
            File.Create(Path.Combine(GetSessionFolder(session), "chat.log"));

            File.WriteAllText()
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
