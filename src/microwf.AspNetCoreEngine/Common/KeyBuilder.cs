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
      return Type.GetType(key);
    }
  }
}