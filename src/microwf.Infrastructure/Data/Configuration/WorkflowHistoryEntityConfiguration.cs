using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure.Configuration
{
  public class WorkflowHistoryEntityConfiguration : IEntityTypeConfiguration<WorkflowHistory>
  {
    public void Configure(EntityTypeBuilder<WorkflowHistory> builder)
    {
      // table
      builder.ToTable("WorkflowHistory");

      // colums
      builder.HasKey(x => x.Id);
      builder.Property(x => x.Created).IsRequired();
      builder.Property(x => x.FromState).IsRequired();
      builder.Property(x => x.ToState).IsRequired();

      // relations

    }
  }
}