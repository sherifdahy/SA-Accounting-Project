using SA.Accounting.Core.Entities.Attachments;

namespace SA.Accounting.Infrastructure.Presistance.Data.Config;

public class AttachmentConfig : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.ContentType)
            .HasMaxLength(100);

        builder.Property(x => x.Note)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Company)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ExpenseClaimItem)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.ExpenseClaimItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CompanyId);

        builder.HasIndex(x => x.ExpenseClaimItemId);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Attachment_FileUrl_NotEmpty",
                "LEN(LTRIM(RTRIM([FileUrl]))) > 0");

            t.HasCheckConstraint(
                "CK_Attachment_FileName_NotEmpty",
                "LEN(LTRIM(RTRIM([FileName]))) > 0");
        });
    }
}
