using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using Vision.Data.Context;
using Vision.Data.Interfaces;
using Vision.Data.Repositories;
using VisionAPI.Indentity;
using VisionAPI.Notifications;

namespace VisionAPI.Configuration;

public static class DependencyInjectionConfig
{
    public static IServiceCollection ResolveDependencies(this IServiceCollection services)
    {
        services.AddScoped<VisionContext>();

        //Repositories
        services.AddScoped<ICameraRepository, CameraRepository>();

        // Notificações
        services.AddScoped<INotificador, Notificador>();

        //User
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IUser, IdentityUser>();
        services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();

        return services;
    }
}
