namespace ASPNETCoreProjectTemplate
{
    public interface IIdentityService
    {
        string GetUserId();

        int GetAppUserId();

        string GetUserName();
    }
}
