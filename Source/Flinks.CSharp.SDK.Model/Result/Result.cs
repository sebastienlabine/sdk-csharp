using System.Collections.Generic;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Result
{
    public class Result
    {
        public Result()
        {
            OperationAccounts = new List<OperationAccount>();
            UsdAccounts = new List<UsdAccount>();
        }

        [JsonProperty("LoginId")]
        public string LoginId { get; set; }

        [JsonProperty("RequestId")]
        public string RequestId { get; set; }

        [JsonProperty("Holder")]
        public Holder Holder { get; set; }

        [JsonProperty("OperationAccounts")]
        public List<OperationAccount> OperationAccounts { get; set; }

        [JsonProperty("USDAccounts")]
        public List<UsdAccount> UsdAccounts { get; set; }

        [JsonProperty("BiggestCreditTrxId")]
        public string BiggestCreditTrxId { get; set; }
    }
}
