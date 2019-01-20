using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Result
{
    public class UsdAccount
    {
        [JsonProperty("Balance")]
        public string Balance { get; set; }
    }
}
