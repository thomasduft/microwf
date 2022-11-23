using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace tomware.Microwf.Domain
{
  public static class ObjectMapper
  {
    public static IEnumerable<WorkItemDto> ToWorkItemViewModelList(
      IEnumerable<WorkItem> items
    )
    {
      return items.Select(i => new WorkItemDto
      {
        Id = i.Id,
        TriggerName = i.TriggerName,
        EntityId = i.EntityId,
        WorkflowType = i.WorkflowType,
        Retries = i.Retries,
        Error = i.Error,
        DueDate = i.DueDate
      });
    }

    public static IEnumerable<WorkflowHistoryDto> ToWorkflowHistoryViewModelList(
      IEnumerable<WorkflowHistory> items
    )
    {
      return items.Select(i => new WorkflowHistoryDto
      {
        Id = i.Id,
        Created = i.Created,
        FromState = i.FromState,
        ToState = i.ToState,
        UserName = i.UserName,
        WorkflowId = i.WorkflowId
      });
    }

    public static IEnumerable<WorkflowVariableDto> ToWorkflowVariableViewModelList(
      IEnumerable<WorkflowVariable> items
    )
    {
      return items.Select(i => new WorkflowVariableDto
      {
        Id = i.Id,
        Type = i.Type,
        Content = i.Content,
        WorkflowId = i.WorkflowId
      });
    }
  }

  public static class PropertyMapper<TInput, TOutput>
  {
    private static readonly Func<TInput, TOutput> cloner;
    private static readonly Action<TInput, TOutput> copier;

    private static readonly IEnumerable<PropertyInfo> sourceProperties;
    private static readonly IEnumerable<PropertyInfo> destinationProperties;

    static PropertyMapper()
    {
      destinationProperties = typeof(TOutput)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(prop => prop.CanWrite);
      sourceProperties = typeof(TInput)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(prop => prop.CanRead);

      cloner = CreateCloner();
      copier = CreateCopier();
    }

    private static Func<TInput, TOutput> CreateCloner()
    {
      //check if type has parameterless constructor - just in case
      if (typeof(TOutput).GetConstructor(Type.EmptyTypes) == null)
        return ((x) => default(TOutput));

      var input = Expression.Parameter(typeof(TInput), "input");

      // For each property that exists in the destination object,
      // is there a property with the same name in the source object?
      var memberBindings = sourceProperties.Join(destinationProperties,
        sourceProperty => sourceProperty.Name,
        destinationProperty => destinationProperty.Name,
        (sourceProperty, destinationProperty) =>
          (MemberBinding)Expression.Bind(destinationProperty,
            Expression.Property(input, sourceProperty)));

      var body = Expression.MemberInit(Expression.New(typeof(TOutput)), memberBindings);
      var lambda = Expression.Lambda<Func<TInput, TOutput>>(body, input);

      return lambda.Compile();
    }

    private static Action<TInput, TOutput> CreateCopier()
    {
      var input = Expression.Parameter(typeof(TInput), "input");
      var output = Expression.Parameter(typeof(TOutput), "output");

      // For each property that exists in the destination object,
      // is there a property with the same name in the source object?
      var memberAssignments = sourceProperties.Join(destinationProperties,
        sourceProperty => sourceProperty.Name,
        destinationProperty => destinationProperty.Name,
        (sourceProperty, destinationProperty) =>
          Expression.Assign(Expression.Property(output, destinationProperty),
            Expression.Property(input, sourceProperty)));

      var body = Expression.Block(memberAssignments);
      var lambda = Expression.Lambda<Action<TInput, TOutput>>(body, input, output);
      return lambda.Compile();
    }

    public static TOutput From(TInput input)
    {
      if (input == null) return default(TOutput);

      return cloner(input);
    }

    public static void CopyTo(TInput input, TOutput output)
    {
      copier(input, output);
    }
  }
}