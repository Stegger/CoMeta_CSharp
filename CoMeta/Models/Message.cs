namespace CoMeta.Models
{
    public class Message
    {
        public long Id { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public string Text { get; set; }
    }
    
}