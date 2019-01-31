using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Score
{
    public class ScoreResult : FlinksRoot
    {
        [JsonProperty("Score")]
        public string Score { get; set; }

        [JsonProperty("Login")]
        public Login Login { get; set; }
    }
}
