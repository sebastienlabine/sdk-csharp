using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Shared
{
    public class FlinksRoot
    {
        [JsonIgnore]
        public ClientStatus ClientStatus { get; set; }
        [JsonProperty("HttpStatusCode", NullValueHandling = NullValueHandling.Ignore)]
        public int HttpStatusCode { get; set; }
        [JsonProperty("Institution")]
        public string Institution { get; set; }
        [JsonProperty("RequestId")]
        public string RequestId { get; set; }
        [JsonProperty("Links")]
        public List<Link> Links { get; set; }
        [JsonProperty("ValidationDetails", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<string>> ValidationDetails { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }
    }
}
