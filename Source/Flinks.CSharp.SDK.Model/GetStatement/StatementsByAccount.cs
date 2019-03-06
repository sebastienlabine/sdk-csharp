using System.Collections.Generic;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.GetStatement
{
    public class StatementsByAccount
    {
        [JsonProperty("AccountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("Statements")]
        public List<Statement> Statements { get; set; }
    }
}
