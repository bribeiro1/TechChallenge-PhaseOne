using Contacts.Application.Services.Contacts;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.Application.DI
{
    public static class ApplicationDI
    {
        public static void AddApplicationDI(this IServiceCollection services)
        {
            services.AddScoped<IContactService, ContactService>();
        }
    }
}