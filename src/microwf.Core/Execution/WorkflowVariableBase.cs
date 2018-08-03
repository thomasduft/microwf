namespace tomware.Microwf.Core
{
  public class WorkflowVariableBase
  {
    public string Type { 
      get {
        return this.GetType().FullName;
      } 
    }
  }
}
