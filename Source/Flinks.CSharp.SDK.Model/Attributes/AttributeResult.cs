using System.Collections.Generic;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Attributes
{
    public class AttributeResult : FlinksRoot
    {
        [JsonProperty("Card")]
        public Dictionary<string, object> Card { get; set; }
        [JsonProperty("Login")]
        public Login Login { get; set; }

    }
}
