using System;

namespace WebApi.Domain
{
  public partial class Holiday
  {
    public int Id { get; set; }
    public string Requestor { get; set; }
    public string Superior { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
  }
}
