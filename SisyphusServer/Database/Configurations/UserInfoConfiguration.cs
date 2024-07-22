using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SisyphusServer.Database.Entities;

namespace SisyphusServer.Database.Configurations {
    public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo> {
        public void Configure(EntityTypeBuilder<UserInfo> builder) {
            builder.ToTable("UserInfos");
            builder.Property(p => p.UserId).HasMaxLength(64).IsRequired(true);
            builder.Property(p => p.Points).IsRequired(true);

            builder.HasKey(p => p.UserId);
        }
    }
}
