namespace ErgometerLibrary
{
    public class ChatMessage
    {
        public string Message { get; }
        public string Name { get; }
        public double TimeStamp { get; set;}

        public ChatMessage(string name, string message)
        {
            Name = name;
            Message = message;
            TimeStamp = Helper.Now;
        }

        public override string ToString()
        {
            return $"[{Name}] <{TimeStamp}> - {Message}";
        }
    }
}