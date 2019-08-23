using System.Collections.Generic;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Attributes
{
    public class Options
    {
        [JsonProperty("AttributesDetail")]
        public List<string> AttributesDetails { get; set; }
    }
}