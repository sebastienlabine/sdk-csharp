using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Shared
{
    public class RequestBody : FlinksRoot
    {
        [JsonIgnore]
        public bool IsAuthenticated => !string.IsNullOrEmpty(LoginId) && !string.IsNullOrEmpty(RequestId);

        [JsonProperty("LoginId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LoginId { get; set; }
        [JsonProperty("DaysOfTransactions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DaysOfTransactions { get; set; }
    }
}
