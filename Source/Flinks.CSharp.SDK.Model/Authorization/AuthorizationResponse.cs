using System.Collections.Generic;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Authorization
{
    public class AuthorizationResponse : FlinksRoot
    {
        public AuthorizationResponse()
        {
            RequestId = null;
        }

        [JsonProperty("Links")]
        public List<Link> Links { get; set; }
        [JsonProperty("SecurityChallenges")]
        public List<SecurityChallenge> SecurityChallenges { get; set; }
        [JsonProperty("Login")]
        public Login Login { get; set; }
    }
}
