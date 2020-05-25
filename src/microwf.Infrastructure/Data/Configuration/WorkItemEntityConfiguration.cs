using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure.Configuration
{
  public class WorkItemEntityConfiguration : IEntityTypeConfiguration<WorkItem>
  {
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
      // table
      builder.ToTable("WorkItem");

      // colums
      builder.HasKey(x => x.Id);
      builder.Property(x => x.TriggerName).IsRequired();
      builder.Property(x => x.EntityId).IsRequired();
      builder.Property(x => x.WorkflowType).IsRequired();
      builder.Property(x => x.DueDate).IsRequired();

      // relations

    }
  }
}