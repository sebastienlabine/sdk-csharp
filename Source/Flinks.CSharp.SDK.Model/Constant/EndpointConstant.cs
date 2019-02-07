namespace Flinks.CSharp.SDK.Model.Constant
{
    public static class EndpointConstant
    {
        public const string BaseUrl = "{Endpoint}/v3/{CustomerId}";
        public const string Authorize = "BankingServices/Authorize";
        public const string GetAccountsSummary = "BankingServices/GetAccountsSummary";
        public const string GetAccountsDetail = "BankingServices/GetAccountsDetail ";
        public const string GetAccountsDetailAsync = "BankingServices/GetAccountsDetailAsync";
        public const string GetStatements = "BankingServices/GetStatements";
        public const string GetStatementsAsync = "BankingServices/GetStatementsAsync";
        public const string SetScheduledRefresh = "BankingServices/SetScheduledRefresh";
        public const string DeleteCard = "BankingServices/DeleteCard";
        public const string GetScore = "Insight/login/{LoginId}/score/{RequestId}";
    }
}
