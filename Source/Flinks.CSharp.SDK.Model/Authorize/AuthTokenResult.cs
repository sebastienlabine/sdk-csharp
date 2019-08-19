using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Authorize
{
    public class AuthTokenResult : FlinksRoot
    {
        [JsonProperty("FlinksCode")]
        public string FlinksCode { get; set; }
        [JsonProperty("Token")]
        public string Token { get; set; }
    }
}
