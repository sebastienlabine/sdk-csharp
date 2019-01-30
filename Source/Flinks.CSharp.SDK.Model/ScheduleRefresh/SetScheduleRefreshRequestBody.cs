using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.ScheduleRefresh
{
    public class SetScheduleRefreshRequestBody
    {
        [JsonProperty("LoginId")]
        public string LoginId { get; set; }
        [JsonProperty("IsActivated")]
        public string IsActivated { get; set; }
    }
}
