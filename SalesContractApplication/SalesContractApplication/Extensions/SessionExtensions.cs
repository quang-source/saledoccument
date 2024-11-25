using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SalesContractApplication.Extensions
{
    public static class SessionExtensions
    {
        // Method to store an object as a JSON string in the session
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Method to retrieve an object from the session, deserializing from JSON
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
