using Newtonsoft.Json;

namespace Employee_TMS.Entities
{
    public static class HttpContextExtensionMethods
    {
        public static void SetComplexData(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetComplexData<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            if (data == null)
            {
                return default;
            }
            else
                return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
