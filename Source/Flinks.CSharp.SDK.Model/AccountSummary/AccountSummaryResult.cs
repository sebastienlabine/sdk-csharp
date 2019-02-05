using System.Collections.Generic;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.AccountSummary
{
    public class AccountSummaryResult : FlinksRoot
    {
        [JsonProperty("Accounts")]
        public List<Account> Accounts { get; set; }
        [JsonProperty("Login")]
        public Login Login { get; set; }
    }
}
