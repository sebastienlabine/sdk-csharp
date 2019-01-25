using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Shared
{
    public class Address
    {
        [JsonProperty("CivicAddress")]
        public string CivicAddress { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("Province")]
        public string Province { get; set; }

        [JsonProperty("PostalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("POBox")]
        public object PoBox { get; set; }

        [JsonProperty("Country")]
        public object Country { get; set; }
    }
}
