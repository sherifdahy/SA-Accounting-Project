using Microsoft.AspNetCore.Identity;
using SA.Accounting.Core.Abstractions.Consts;
using SA.Accounting.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA.Accounting.Infrastructure.Presistance.Data.Config;

public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(256);
        builder.Property(x => x.SSN).HasMaxLength(256);

        builder.HasData(new ApplicationUser()
        {
            Id = DefaultUsers.AdminId,
            SSN = DefaultUsers.SSN,
            UserName = DefaultUsers.AdminEmail,
            Email = DefaultUsers.AdminEmail,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
            ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
            SecurityStamp = DefaultUsers.AdminSecurityStamp,
            EmailConfirmed = true,
            PasswordHash = DefaultUsers.AdminPassword
        });
    }
}
