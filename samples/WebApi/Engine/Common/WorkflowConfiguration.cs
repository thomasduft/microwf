using System.Collections.Generic;

namespace tomware.Microwf.Engine
{
  public class Workflow
  {
    public string Type { get; set; }
    public string StartUrl { get; set; }
    public string Description { get; set; }
  }

  public class WorkflowConfiguration
  {
    public List<Workflow> Types { get; set; }
  }
}