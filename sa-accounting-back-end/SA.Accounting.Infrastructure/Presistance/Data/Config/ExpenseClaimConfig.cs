using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Infrastructure.Presistance.Data.Config;

public class ExpenseClaimConfig : IEntityTypeConfiguration<ExpenseClaim>
{
    public void Configure(EntityTypeBuilder<ExpenseClaim> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Number)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Number)
            .IsUnique();

        builder.Property(x => x.Note)
            .HasMaxLength(1000);

        builder.Property(x => x.ClaimDate)
            .IsRequired();

        builder.Property(x => x.CurrentState)
            .IsRequired()
            .HasDefaultValue(ExpenseClaimState.Draft);

        builder.HasIndex(x => new { x.UserId, x.ClaimDate });

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_ExpenseClaim_State_Valid",
                $"[{nameof(ExpenseClaim.CurrentState)}] IN ({string.Join(",", Enum.GetValues<ExpenseClaimState>().Select(x => (int)x))})");
        });

        // audiance entity
        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedBy)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
