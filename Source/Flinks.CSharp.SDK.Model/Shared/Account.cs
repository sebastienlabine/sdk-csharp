﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Shared
{
    public class Account
    {
        [JsonProperty("EftEligibleRatio")]
        public long EftEligibleRatio { get; set; }

        [JsonProperty("Transactions")]
        public List<Transaction> Transactions { get; set; }

        [JsonProperty("TransitNumber")]
        public long? TransitNumber { get; set; }

        [JsonProperty("InstitutionNumber")]
        public long? InstitutionNumber { get; set; }

        [JsonProperty("OverdraftLimit", NullValueHandling = NullValueHandling.Ignore)]
        public long? OverdraftLimit { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("AccountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("Balance")]
        public Balance Balance { get; set; }

        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Currency")]
        public string Currency { get; set; }

        [JsonProperty("Holder")]
        public Holder Holder { get; set; }

        [JsonProperty("Id")]
        public Guid Id { get; set; }
    }
}