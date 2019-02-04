using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Authorize
{
    public class SecurityChallenge
    {
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("Prompt")]
        public string Prompt { get; set; }
        [JsonIgnore]
        public string Answer { get; set; }
    }
}
