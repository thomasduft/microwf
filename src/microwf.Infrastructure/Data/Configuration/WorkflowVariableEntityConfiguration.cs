using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure.Configuration
{
  public class WorkflowVariableEntityConfiguration : IEntityTypeConfiguration<WorkflowVariable>
  {
    public void Configure(EntityTypeBuilder<WorkflowVariable> builder)
    {
      // table
      builder.ToTable("WorkflowVariable");

      // colums
      builder.HasKey(x => x.Id);
      builder.Property(x => x.Type).IsRequired();
      builder.Property(x => x.Content).IsRequired();

      // relations

    }
  }
}