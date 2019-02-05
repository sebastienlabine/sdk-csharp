namespace Flinks.CSharp.SDK.Model.Shared
{
    public enum ClientStatus
    {
        UNKNOWN = 0,
        ERROR = 1,
        PENDING_MFA_ANSWERS = 2,
        AUTHORIZED = 3,
        UNAUTHORIZED = 4
    }
}
