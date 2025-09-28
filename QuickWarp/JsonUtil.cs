using System.IO;
using Newtonsoft.Json;

namespace QuickWarp;

public static class JsonUtil
{
    public static readonly JsonSerializer _js;

    public static T Deserialize<T>(string embeddedResourcePath)
    {
        using StreamReader sr = new(typeof(JsonUtil).Assembly.GetManifestResourceStream(embeddedResourcePath));
        using JsonTextReader jtr = new(sr);
        return _js.Deserialize<T>(jtr);
    }
    
    static JsonUtil()
    {
        _js = new JsonSerializer
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
        };
    }
}
