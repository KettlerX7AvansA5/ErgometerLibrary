namespace ErgometerLibrary
{
    public class ChatMessage
    {
        public string Message { get; }
        public string Name { get; }
        public double TimeStamp { get; set;}
        public bool IsDoctor { get; set; }

        public ChatMessage(string name, string message, bool isDoctor)
        {
            Name = name;
            Message = message;
            TimeStamp = Helper.Now;
            IsDoctor = isDoctor;
        }

        public override string ToString()
        {
            return $"[{Name}] <{TimeStamp}> - {Message}";
        }
    }
}