using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XMAS2019.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Reindeer
    {
        Dasher, 
        Dancer, 
        Prancer, 
        Vixen, 
        Comet, 
        Cupid, 
        Donner,
        Blitzen
    }
}