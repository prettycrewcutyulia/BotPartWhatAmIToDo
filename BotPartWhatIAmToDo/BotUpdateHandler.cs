using System.Text;
using Newtonsoft.Json;
using ServerPartWhatAmItOdO.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ServerPartWhatAmItOdO;

public class BotUpdateHandler
{
    private static readonly HttpClient httpClient = new();

    public static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var message = update.Message;
        if (message is { Text: not null } && message.Text.StartsWith("/start"))
        {
            string[] parts = message.Text.Split(" ");
            if (parts.Length > 1)
            {
                string startParam = parts[1];
                long chatId = message.Chat.Id;
                Console.WriteLine($"Получен параметр запуска: {startParam}");
                Console.WriteLine($"ChatId: {chatId}");

                // Отправляем запрос на сервер
                SendRequestToServerModel model = new SendRequestToServerModel(chatId, startParam);
                bool serverResponse = await SendRequestToServer(model);

                // Обрабатываем ответ сервера
                if (serverResponse)
                {
                    await client.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Сервер подтвердил успех операции.",
                        cancellationToken: token
                    );
                }
                else
                {
                    await client.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Произошла ошибка при взаимодействии с сервером.",
                        cancellationToken: token
                    );
                }
            }
            else
            {
                await client.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Укажите параметр для команды /start.",
                    cancellationToken: token
                );
            }
        }
    }

    private static async Task<bool> SendRequestToServer(SendRequestToServerModel request)
    {
        Console.WriteLine("Ky");
        try
        {
            DotNetEnv.Env.Load();
            string url = Environment.GetEnvironmentVariable("SERVER_URL");
            // Получение токена доступа (предположительно из переменных окружения)
            string accessToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN");
            
            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            
            // Сериализация объекта запроса в JSON
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Выполнение PUT-запроса
            var response = await httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // Вернуть содержимое ошибки для дополнительной информации
            return false;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Ошибка при отправке запроса: {ex.Message}");
            return false;
        }
    }
}