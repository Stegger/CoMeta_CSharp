using MySecurity.Authorization;

namespace CoMeta.Models
{
    public class Message : IAuthorizableOwnerIdentity
    {
        
        
        public long Id { get; set; }
        public long SenderId { get; set; }
        public string SendeName { get; set; }
        public long ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string Text { get; set; }
        public long getAuthorizedOwnerId()
        {
            return ReceiverId;
        }

        public string getAuthorizedOwnerName()
        {
            return ReceiverName;
        }
    }
    
}