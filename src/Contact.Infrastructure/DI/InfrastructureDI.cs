using Contacts.Infrastructure.Repositories.Contacts;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.Infrastructure.DI;

public static class InfrastructureDI
{
    public static void AddInfrastructureDI(this IServiceCollection services)
    {
        services.AddScoped<IContactRepository, ContactRepository>();
    }
}