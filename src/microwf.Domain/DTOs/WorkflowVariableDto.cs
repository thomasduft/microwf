namespace tomware.Microwf.Domain
{
  public class WorkflowVariableDto
  {
    public int Id { get; set; }

    public string Type { get; set; }

    public string Content { get; set; }

    public int WorkflowId { get; set; }
  }
}