using Hotvenues.Models;
using System;

namespace hotvenues.Models
{
    public class CommentStatus : HasId
    {
        public LiveStatus Status { get; set; }
        public long StatusId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }

}
