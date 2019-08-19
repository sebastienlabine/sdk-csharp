using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Score
{
    public class ScoreRequestBody
    {
        [JsonProperty("LoanAmount")]
        public string LoanAmount { get; set; }

        [JsonProperty("UserPersonalInformation")]
        public UserPersonalInformation UserPersonalInformation { get; set; }
    }
}
