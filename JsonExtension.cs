using Newtonsoft.Json;

public static class JsonExtension
{
    public static string Serialize<TSource>(this TSource Source)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        var Json = JsonConvert.SerializeObject(Source, settings);
        return Json;
    }

    public static TDest Deserialize<TDest>(this string Json)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        var Result = JsonConvert.DeserializeObject<TDest>(Json, settings);
        return Result;
    }

    public static TDest Deserialize<TDest>(this object Source)
    {
        var Json = Source.Serialize();
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        Json = JsonConvert.DeserializeObject(Json, settings).ToString();

        var Result = JsonConvert.DeserializeObject<TDest>(Json, settings);
        return Result;
    }

    public static List<TDest> DeserializeToList<TDest>(this object Source)
    {
        var Json = Source.Serialize();
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //settings.DateFormatString = "dd/MM/yyyy";
        Json = JsonConvert.DeserializeObject(Json, settings).ToString();
        var Result = JsonConvert.DeserializeObject<List<TDest>>(Json, settings);
        return Result;
    }
}