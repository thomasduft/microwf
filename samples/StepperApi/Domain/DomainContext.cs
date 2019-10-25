using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Engine;
using StepperApi.Workflows.Stepper;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;

namespace StepperApi.Domain
{
  public class DomainContext : EngineDbContext
  {
    public DbSet<Stepper> Steppers { get; set; }

    public DomainContext(DbContextOptions<DomainContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // see: https://stackoverflow.com/questions/52684458/updating-entity-in-ef-core-application-with-sqlite-gives-dbupdateconcurrencyexce
      if (Database.IsSqlite())
      {
        var timestampProperties = modelBuilder.Model
          .GetEntityTypes()
          .SelectMany(t => t.GetProperties())
          .Where(p => p.ClrType == typeof(byte[])
            && p.ValueGenerated == ValueGenerated.OnAddOrUpdate
            && p.IsConcurrencyToken);

        foreach (var property in timestampProperties)
        {
          property.SetValueConverter(new SqliteTimestampConverter());
          // property.GetTypeMapping() .SetDefaultValue("CURRENT_TIMESTAMP");
        }
      }
    }
  }

  public class SqliteTimestampConverter : ValueConverter<byte[], string>
  {
    public SqliteTimestampConverter() : base(
        v => v == null ? null : ToDb(v),
        v => v == null ? null : FromDb(v))
    { }
    public static byte[] FromDb(string v) =>
        v.Select(c => (byte)c).ToArray(); // Encoding.ASCII.GetString(v)

    public static string ToDb(byte[] v) =>
        new string(v.Select(b => (char)b).ToArray()); // Encoding.ASCII.GetBytes(v))
  }
}
