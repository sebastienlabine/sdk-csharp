using System;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Score
{
    public class UserPersonalInformation
    {
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Sex")]
        public Sex Sex { get; set; }

        [JsonProperty("BirthDate")]
        public string BirthDate { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("SocialInsuranceNumber")]
        public string SocialInsuranceNumber { get; set; }

        [JsonProperty("Address")]
        public Address Address { get; set; }
    }
}
