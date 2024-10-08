using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using VisionAPI.Middlewares;

namespace VisionAPI.Configuration;

public static class ApiConfig
{
    public static IServiceCollection WebApiConfig(this IServiceCollection services)
    {

        services.Configure<ApiBehaviorOptions>(opt =>
        {
            opt.SuppressModelStateInvalidFilter = true;
        });

        services.AddCors(options =>
        {
            options.AddPolicy("Development", builder =>
                     builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .DisallowCredentials());

            options.AddPolicy("Production", builder =>
                     builder.WithMethods("GET", "POST", "PUT", "DELETE")
                            .WithOrigins(new[] { "https://localhost:8100" })
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .AllowAnyHeader()
                            .AllowCredentials());
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static IApplicationBuilder UseAppConfiguration(this IApplicationBuilder app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseAntiXssMiddleware();

        app.UseStaticFiles();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        return app;
    }
}
