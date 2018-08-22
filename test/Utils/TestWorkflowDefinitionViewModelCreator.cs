using System;
using System.Collections.Generic;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;

namespace microwf.Tests.Utils
{
  public class TestWorkflowDefinitionViewModelCreator : IWorkflowDefinitionViewModelCreator
  {
    public WorkflowDefinitionViewModel CreateViewModel(string type)
    {
      return new WorkflowDefinitionViewModel
      {
        Type = type,
        Title = "Title",
        Route = "Route",
        Description = "Description"
      };
    }
  }
}