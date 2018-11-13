using System.Collections.Generic;

namespace tomware.Microwf.Engine
{
  public class WorkflowConfiguration
  {
    public List<WorkflowType> Types { get; set; }
  }

  public class WorkflowType
  {
    public string Type { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Route { get; set; }
  }
}