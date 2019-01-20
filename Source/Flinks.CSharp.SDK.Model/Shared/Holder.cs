using Flinks.CSharp.SDK.Model.Account;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Shared
{
    public class Holder
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Address")]
        public Address Address { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; } 
    }
}
