using CasCap.Services;
namespace CasCap;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();
        services.AddSingleton<IPriceGeneratorService, PriceGeneratorService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<GreeterService>();
            endpoints.MapGrpcService<MarketsService>();

            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
    }
}