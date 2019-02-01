using System.Collections.Generic;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Model.Statement
{
    public class StatementResult : FlinksRoot
    {
        [JsonProperty("StatementsByAccount")]
        public List<StatementsByAccount> StatementsByAccount { get; set; }
    }
}
