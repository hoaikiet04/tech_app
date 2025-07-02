namespace MemoryImage.Models
{
    public enum FriendshipStatus
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2,
        Blocked = 3
    }

    public class Friendship
    {
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public int ReceiverId { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResponsedAt { get; set; }
        
        // Navigation properties
        public virtual User Requester { get; set; }
        public virtual User Receiver { get; set; }
    }
}