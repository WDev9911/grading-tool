using GradingTool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradingTool.Infrastructure.Persistence.Configurations;

public class CriterionConfiguration : IEntityTypeConfiguration<Criterion>
{
    public void Configure(EntityTypeBuilder<Criterion> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => new { c.QuestionId, c.Code }).IsUnique();

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(c => c.MaxScore)
            .HasColumnType("decimal(4,2)")
            .IsRequired();

        builder.Property(c => c.OrderIndex).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();
    }
}
