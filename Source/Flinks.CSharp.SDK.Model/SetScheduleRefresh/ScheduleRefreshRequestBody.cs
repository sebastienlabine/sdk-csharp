using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.SetScheduleRefresh
{
    public class ScheduleRefreshRequestBody
    {
        [JsonProperty("LoginId")]
        public string LoginId { get; set; }
        [JsonProperty("IsActivated")]
        public string IsActivated { get; set; }
    }
}
