using System;
using System.ComponentModel.DataAnnotations;

namespace Hotvenues.Models
{
    public interface IHasId
    {
        [Key]
        long Id { get; set; }
    }

    public interface ISecured
    {
        bool Locked { get; set; }
        bool Hidden { get; set; }
    }

    public interface IAuditable : IHasId
    {
        [Required]
        string CreatedBy { get; set; }
        [Required]
        string ModifiedBy { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime ModifiedAt { get; set; }
    }

    public class HasId : IHasId
    {
        public long Id { get; set; }
    }

    public class AuditFields : HasId, IAuditable, ISecured
    {
        [Required]
        public string CreatedBy { get; set; }
        [Required]
        public string ModifiedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool Locked { get; set; }
        public bool Hidden { get; set; }
    }

    public class LookUp : HasId
    {
        [MaxLength(512), Required]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
    }


    public class AuditTrail : HasId
    {
        public DateTime Timestamp { get; set; }
        public string Model { get; set; }
        public string ReferenceId { get; set; }
        public AuditAction Action { get; set; }
        public string OldObject { get; set; }
        public string NewObject { get; set; }
        public string User { get; set; }
    }

    public enum AuditAction
    {
        Read,
        Create,
        Update,
        Delete
    }

    public class AuditFlag: HasId
    {
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public FlagType Type { get; set; }
        public long ReferenceId { get; set; }
        public FlagStatus Status { get; set; }
        public string Notes { get; set; }        
        public string User { get; set; }
        public string Auditor { get; set; }
    }

    public enum FlagType
    {
        StockDiscrepancy,
        InvalidAdjustment
    }

    public enum FlagStatus
    {
        New,
        Resolved,
        Ignored
    }

}
