using Newtonsoft.Json;

namespace tomware.Microwf.Engine
{
  public class LogHelper
  {
    public static string SerializeObject<T>(T obj)
    {
      return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
      {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
      });
    }
  }
}