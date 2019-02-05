using System.Collections.Generic;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.AccountsDetail
{
    public class AccountsDetailResult : FlinksRoot
    {
        [JsonProperty("Accounts")]
        public List<Account> Accounts { get; set; }

        [JsonProperty("Login")]
        public Login Login { get; set; }

    }
}
