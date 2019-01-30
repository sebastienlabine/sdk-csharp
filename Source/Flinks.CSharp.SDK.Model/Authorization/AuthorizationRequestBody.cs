using System.Collections.Generic;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Authorization
{
    public class AuthorizationRequestBody : FlinksRoot
    {
        public AuthorizationRequestBody()
        {
            Save = null;
            RequestId = null;
        }

        [JsonProperty("UserName")]
        public string UserName { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }
        [JsonProperty("LoginId")]
        public string LoginId { get; set; }
        [JsonProperty("SecurityResponses")]
        public Dictionary<string, List<string>> SecurityResponses { get; set; }
        [JsonProperty("Save")]
        public string Save { get; set; }
        [JsonProperty("MostRecentCached")]
        public string MostRecentCached { get; set; }
        [JsonProperty("WithMfaQuestions")]
        public string WithMfaQuestions { get; set; }
        [JsonProperty("Language")]
        public string Language { get; set; }
        [JsonProperty("Tag")]
        public string Tag { get; set; }
        [JsonProperty("ScheduleRefresh")]
        public string ScheduleRefresh { get; set; }
    }
}
