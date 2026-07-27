using SA.Accounting.Core.Entities.Custodies;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Infrastructure.Presistance.Data.Config;

public class MovementConfig : IEntityTypeConfiguration<Movement>
{
    public void Configure(EntityTypeBuilder<Movement> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.DateTime)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Note)
            .HasMaxLength(500);

        builder.HasOne(x => x.ExpenseClaim)
            .WithOne(x => x.SettlementMovement)
            .HasForeignKey<Movement>(x => x.ExpenseClaimId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ExpenseClaimId, x.Type })
            .IsUnique()
            .HasFilter("[ExpenseClaimId] IS NOT NULL AND [Type] = 2");

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Movement_Amount_Positive",
                "[Amount] > 0");

            t.HasCheckConstraint(
                "CK_Movement_Type_Valid",
                "[Type] IN (1, 2, 3, 4, 5)");

            t.HasCheckConstraint(
                "CK_Movement_ExpenseClaim_Rule",
                @"(
                ([Type] = 2 AND [ExpenseClaimId] IS NOT NULL)
                OR
                ([Type] <> 2 AND [ExpenseClaimId] IS NULL)
            )");
        });
    }
}
