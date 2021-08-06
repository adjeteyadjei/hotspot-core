using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace Hotvenues.Models
{
    public class AppSetting : HasId
    {
        [MaxLength(512), Required]
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class AppSettingConfiguration : IEntityTypeConfiguration<AppSetting>
    {
        public void Configure(EntityTypeBuilder<AppSetting> builder)
        {
            builder.HasIndex(q => q.Name).IsUnique();
        }
    }
}