using Hotvenues.Models;
using System;
using System.Collections.Generic;

namespace hotvenues.Models
{
    public class LiveStatus : HasId
    {
        public DateTime Timestamp { get; set; }
        public string VendorId { get; set; }
        public User Vendor { get; set; }
        public string Media { get; set; }
        public string MediaExtension { get; set; }
        public string Caption { get; set; }
        public ICollection<LikeStatus> Likes { get; set; }
        public ICollection<CommentStatus> Comments { get; set; }
    }

}
