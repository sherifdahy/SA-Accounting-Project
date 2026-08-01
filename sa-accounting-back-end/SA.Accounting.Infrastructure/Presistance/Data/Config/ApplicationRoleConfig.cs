using SA.Accounting.Core.Abstractions.Consts;
using SA.Accounting.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA.Accounting.Infrastructure.Presistance.Data.Config;

public class ApplicationRoleConfig : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(new ApplicationRole()
        {
            Id = DefaultRoles.AdminRoleId,
            Name = DefaultRoles.Admin,
            NormalizedName = DefaultRoles.Admin.ToUpper(),
            ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
        },
        new ApplicationRole()
        {
            Id = DefaultRoles.EmployeeRoleId,
            Name = DefaultRoles.Employee,
            NormalizedName = DefaultRoles.Employee.ToUpper(),
            ConcurrencyStamp = DefaultRoles.EmployeeRoleConcurrencyStamp
        });
    }
}
