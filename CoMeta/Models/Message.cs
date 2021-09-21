namespace CoMeta.Models
{
    public class Message
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public string Text { get; set; }
    }
}