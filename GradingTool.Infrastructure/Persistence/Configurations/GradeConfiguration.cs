using GradingTool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradingTool.Infrastructure.Persistence.Configurations;

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Score)
            .HasColumnType("decimal(4,2)")
            .IsRequired();

        builder.Property(g => g.Comment)
            .HasMaxLength(1000);

        builder.Property(g => g.UpdatedAt)
            .IsRequired();

        builder.HasIndex(g => new { g.SubmissionId, g.CriterionId }).IsUnique();

        builder.HasOne(g => g.Criterion)
            .WithMany(c => c.Grades)
            .HasForeignKey(g => g.CriterionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
