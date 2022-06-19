﻿using Telegram.Bot;
using KAIFreeAudiencesBot.Models;
using KAIFreeAudiencesBot.Services;
using KAIFreeAudiencesBot.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace KAIFreeAudiencesBot;

public class Startup
{
    public IConfiguration Configuration { get; }
    private BotConfiguration BotConfiguration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        BotConfiguration = Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();

        using var client = new SchDbContext();
        client.Database.EnsureCreated();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var httpClient = new HttpClient();
        services.AddHttpClient("kfab_webhook").AddTypedClient<ITelegramBotClient>(client =>
            new TelegramBotClient(BotConfiguration.BotApiKey, httpClient));
        
        
        services.AddEntityFrameworkSqlite().AddDbContext<SchDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("ScheduleConnectionSqlite")));
        services.AddHostedService<ConfigureWebhook>();
        services.AddScoped<HandleUpdateService>();
        services.AddControllers().AddNewtonsoftJson();
        services.AddControllers();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        //app.UseHttpsRedirection();

        app.UseEndpoints(endpoints =>
        {
            var token = BotConfiguration.BotApiKey;
            endpoints.MapControllerRoute(
                name: "tgbot",
                pattern: $"bot/{token}",
                new {controller = "Webhook", action = "Post"});
        });
    }
}
