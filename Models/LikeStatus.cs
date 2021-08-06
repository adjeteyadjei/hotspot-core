using Hotvenues.Models;
using System;

namespace hotvenues.Models
{
    public class LikeStatus : HasId
    {
        public LiveStatus Status { get; set; }
        public long StatusId { get; set; }
        public DateTime Timestamp { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }

}
