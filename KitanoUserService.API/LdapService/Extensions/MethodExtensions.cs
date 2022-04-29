using Microsoft.Extensions.DependencyInjection;

namespace KitanoUserService.API.LdapService.Extensions
{
    public static class MethodExtensions
    {
        public static void AddLdapService(this IServiceCollection services)
            => services.AddTransient<Interface.ILdapService, Service.LdapService>();
    }
}
