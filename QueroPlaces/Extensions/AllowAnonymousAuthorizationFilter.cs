using Hangfire.Dashboard;

namespace QueroPlaces.Extensions;

public class AllowAnonymousAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // Permitir acesso anônimo ao dashboard do Hangfire em todos os ambientes
        return true;
    }
}