namespace NsqSharp.Core
{
    internal class MessageResponse
    {
        public Message msg { get; set; }
        public Command cmd { get; set; }
        public bool success { get; set; }
        public bool backoff { get; set; }
    }
}