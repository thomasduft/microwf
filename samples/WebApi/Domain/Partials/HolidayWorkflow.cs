using tomware.Microwf;

namespace WebApi.Domain
{
  public partial class Holiday : IWorkflow
  {
    public string Type { get; set; }

    public string State { get; set; }
  }
}
