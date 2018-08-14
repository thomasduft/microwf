using System;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public abstract class EntityWorkflowDefinitionBase : WorkflowDefinitionBase
  {
    public abstract Type EntityType { get; }


    public string ToKey(Type type)
    {
      return KeyBuilder.ToKey(type);
    }

    public Type FromKey(string key)
    {
      return KeyBuilder.FromKey(key);
    }
  }
}