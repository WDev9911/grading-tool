using GradingTool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradingTool.Infrastructure.Persistence.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasIndex(s => s.StudentCode).IsUnique();

        builder.Property(s => s.StudentCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.StudentName)
            .IsRequired()
            .HasMaxLength(100)
            .UseCollation("Vietnamese_CI_AS");

        builder.Property(s => s.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(s => s.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.UploadedAt).IsRequired();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.TotalScore)
            .HasColumnType("decimal(4,2)");

        builder.Property(s => s.GeneralComment)
            .HasMaxLength(2000);

        builder.HasMany(s => s.Grades)
            .WithOne(g => g.Submission)
            .HasForeignKey(g => g.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
