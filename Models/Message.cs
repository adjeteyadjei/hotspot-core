using System;
using System.ComponentModel.DataAnnotations;

namespace Hotvenues.Models
{
    public class Message : HasId
    {
        [MaxLength(256), Required]
        public string Recipient { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(128)]
        public string Subject { get; set; }
        [Required]
        public string Text { get; set; }
        public MessageStatus Status { get; set; }
        public MessageType Type { get; set; }
        [MaxLength(5000)]
        public string Response { get; set; }
        public DateTime TimeStamp { get; set; }
    }

		public class MessageOutward : HasId
    {
        [MaxLength(256), Required]
        public string Recipient { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(128)]
        public string Subject { get; set; }
        [Required]
        public string Text { get; set; }
        public bool Processed { get; set; }
        public MessageType Type { get; set; }
    }

		public enum MessageType
		{
				// ReSharper disable once InconsistentNaming
				SMS,
				Email
		}

		public enum MessageStatus
    {
        Sent,
        Received,
        Failed
    }
}