using System;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public abstract class EntityWorkflowDefinitionBase : WorkflowDefinitionBase
  {
    public abstract Type EntityType { get; }
  }
}