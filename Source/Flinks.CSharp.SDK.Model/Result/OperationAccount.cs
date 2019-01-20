using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Result
{
    public class OperationAccount
    {
        [JsonProperty("AccountNumber")]
        public string AccountNumber { get; set; }
    }
}
