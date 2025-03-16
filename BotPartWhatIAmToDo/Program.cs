using Telegram.Bot;
using Telegram.Bot.Polling;
using DotNetEnv;
using Microsoft.OpenApi.Models;

class Program
{
    static void Main(string[] args)
    {
        startup(args);
        // Получите токен из переменных окружения
        Env.Load();
        string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException("TELEGRAM_BOT_TOKEN не может быть пустым");
        }
        using var cts = new CancellationTokenSource();
        var client = new TelegramBotClient(token, cancellationToken: cts.Token);
        client.StartReceiving(BotUpdateHandler.Update, Error);
        Console.ReadLine();
    }    

    private static async Task Error(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
    {
    }

    private static void startup(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Добавление контроллеров
        builder.Services.AddControllers();

// Добавление Swagger
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "My API",
                Description = "A simple example ASP.NET Core Web API"
            });
        });

        var app = builder.Build();

// Конфигурация HTTP-конвейера
        if (app.Environment.IsDevelopment())
        {
            // Включение Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WhatIAmToDo");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}