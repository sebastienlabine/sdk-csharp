using System;
using System.Collections.Generic;

namespace Flinks.CSharp.SDK.Model.GetStatement
{
    public class StatementsRequestBody
    {
        public string RequestId { get; set; }
        public string NumberOfStatements { get; set; }
        public List<Guid> AccountsFilter { get; set; }
    }
}
