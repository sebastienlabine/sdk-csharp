using System.Collections.Generic;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.AccountDetail
{
    public class GetAccountDetailRequestBody
    {
        [JsonProperty("RequestId")]
        public string RequestId { get; set; }
        [JsonProperty("WithAccountIdentity")]
        public bool? WithAccountIdentity { get; set; }
        [JsonProperty("WithKyc")]
        public bool? WithKyc { get; set; }
        [JsonProperty("WithTransactions")]
        public bool? WithTransactions { get; set; }
        [JsonProperty("WithBalance")]
        public bool? WithBalance { get; set; }
        [JsonProperty("DaysOfTransaction")]
        public string DaysOfTransaction { get; set; }
        [JsonProperty("AccountsFilter")]
        public List<string> AccountsFilter { get; set; }
    }
}
