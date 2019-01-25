using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Shared
{
    public class Balance
    {
        [JsonProperty("Available")]
        public double? Available { get; set; }

        [JsonProperty("Current")]
        public double? Current { get; set; }

        [JsonProperty("Limit")]
        public double? Limit { get; set; }
    }
}
