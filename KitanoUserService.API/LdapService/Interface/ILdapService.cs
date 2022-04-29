using KitanoUserService.API.LdapService.Entity;

namespace KitanoUserService.API.LdapService.Interface
{
    public interface ILdapService
    {
        LdapUser Login(string userName, string password);
        LdapUser FindUser(string userName);
    }
}
