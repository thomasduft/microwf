using System;

namespace tomware.Microwf.Engine
{
  internal static class KeyBuilder
  {
    public static string ToKey(Type type)
    {
      return type.FullName;
    }

    public static Type FromKey(string key)
    {
      var type = Type.GetType(key);
      if (type != null) return type;

      foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
      {
        type = a.GetType(key);
        if (type != null) return type;
      }

      return null;
    }
  }
}