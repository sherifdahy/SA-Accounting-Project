using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Enums;

namespace SA.Accounting.Infrastructure.Presistance.Data.Config;

public class ExpenseClaimItemConfig : IEntityTypeConfiguration<ExpenseClaimItem>
{
    public void Configure(EntityTypeBuilder<ExpenseClaimItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Note)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.State)
            .IsRequired()
            .HasDefaultValue(ExpenseClaimItemState.Pending);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.ExpenseClaimId);

        builder.HasIndex(x => x.CompanyId);

        builder.HasIndex(x => x.ExpenseCategoryId);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_ExpenseClaimItem_Amount_Positive",
                "[Amount] > 0");

            t.HasCheckConstraint(
                "CK_ExpenseClaimItem_State_Valid",
                $"[{nameof(ExpenseClaimItem.State)}] IN ({string.Join(',', Enum.GetValues<ExpenseClaimItemState>().Select(x => (int)x))})");

            t.HasCheckConstraint(
                "CK_ExpenseClaimItem_RejectionReason_Required_WhenRejected",
                $@"
                (
                    [State] <> {(int)ExpenseClaimItemState.Rejected}
                    OR
                    (
                        [RejectionReason] IS NOT NULL
                        AND LEN(LTRIM(RTRIM([RejectionReason]))) > 0
                    )
                )");
        });
    }
}
