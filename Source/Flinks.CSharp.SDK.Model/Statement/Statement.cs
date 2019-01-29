using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Statement
{
    public class Statement
    {
        [JsonProperty("UniqueId")]
        public string UniqueId { get; set; }

        [JsonProperty("FileType")]
        public string FileType { get; set; }

        [JsonProperty("Base64Bytes")]
        public string Base64Bytes { get; set; }
    }
}
