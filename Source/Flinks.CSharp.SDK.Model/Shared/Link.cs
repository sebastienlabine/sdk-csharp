using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Shared
{
    public class Link
    {
        [JsonProperty("rel")]
        public string Rel { get; set; }
        [JsonProperty("href")]
        public string Href { get; set; }
        [JsonProperty("example")]
        public object Example { get; set; }
    }
}
