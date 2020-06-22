using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure.Configuration
{
  public class WorkflowEntityConfiguration : IEntityTypeConfiguration<Workflow>
  {
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
      // table
      builder.ToTable("Workflow");

      // colums
      builder.HasKey(x => x.Id);
      builder.Property(x => x.Type).IsRequired();
      builder.Property(x => x.State).IsRequired();
      builder.Property(x => x.CorrelationId).IsRequired();

      // relations
      // builder.HasOne(x => x.WorkflowHistories).WithMany().OnDelete(DeleteBehavior.Cascade);
      // builder.HasOne(x => x.WorkflowVariables).WithMany().OnDelete(DeleteBehavior.Cascade);
    }
  }
}