using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace XMAS2019.Domain.Infrastructure
{
    public static class MiscExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }

        public static T DeepClone<T>(this T source)
        {
            if (ReferenceEquals(source, null)) 
                return default;

            var deserializeSettings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };

            var serializeObject = JsonConvert.SerializeObject(source);

            return JsonConvert.DeserializeObject<T>(serializeObject, deserializeSettings);
        }
    }
}