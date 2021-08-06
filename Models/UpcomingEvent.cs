using Hotvenues.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hotvenues.Models
{
    public class UpcomingEvent : HasId
    {
        public string VendorId { get; set; }
        public User Vendor { get; set; }
        public string Media { get; set; }
        public string MediaExtension { get; set; }
        public string Theme { get; set; }
        public string Location { get; set; }
        public DateTime DateTime { get; set; }
    }
}
