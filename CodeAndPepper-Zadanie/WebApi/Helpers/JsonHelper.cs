using Newtonsoft.Json;

namespace WebApi.Helpers
{
    public class JsonHelper<T>
    {
        public static string JsonConverter(T model, string name = null)
        {
            string json;
            if (name == null)
            {
                json = JsonConvert.SerializeObject(model,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            }
            else
            {
                json = "{\"" + name + "\":" + JsonConvert.SerializeObject(model,
                                Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                }) + "}";
            }
            return json;
        }
    }
}
