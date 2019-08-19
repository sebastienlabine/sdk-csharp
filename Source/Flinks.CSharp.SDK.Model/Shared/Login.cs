using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Shared
{
    public class Login
    {
        [JsonProperty("Username")]
        public string Username { get; set; }
        [JsonProperty("IsScheduledRefresh")]
        public string IsScheduledRefresh { get; set; }
        [JsonProperty("LastRefresh")]
        public string LastRefresh { get; set; }
        [JsonProperty("Id")]
        public string Id { get; set; }
    }
}
