using System.Collections.Generic;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Account
{
    public class AccountDetail : FlinksRoot
    {
        [JsonProperty("Accounts")]
        public List<Account> Accounts { get; set; }

        [JsonProperty("Login")]
        public Login Login { get; set; }

    }
}
