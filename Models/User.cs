using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotvenues.Models
{
    public class User : IdentityUser
    {
        [MaxLength(128), Required]
        public string Name { get; set; }
        public string Picture { get; set; }
        public virtual Profile Profile { get; set; }
        public long ProfileId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool Locked { get; set; }
        public bool Hidden { get; set; }
        public UserType Type { get; set; }
        public string VendorLocation { get; set; }
    }
    
    public enum Gender
    {
        Male,
        Female
    }

    public enum UserType
    {
        Vendor,
        User
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //builder.HasMany(q => q.Locations);

            //.WithOne(q => q.Users)
            //.OnDelete(DeleteBehavior.Cascade);
        }
    }
}
