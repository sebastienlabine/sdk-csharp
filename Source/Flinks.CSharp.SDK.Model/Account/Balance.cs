using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Account
{
    public class Balance
    {
        [JsonProperty("Available")]
        public object Available { get; set; }

        [JsonProperty("Current")]
        public long Current { get; set; }

        [JsonProperty("Limit")]
        public object Limit { get; set; }
    }
}
