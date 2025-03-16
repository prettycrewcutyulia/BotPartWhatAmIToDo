using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text;
using System.Text.Json;

public class BotUpdateHandler
{
    private static readonly HttpClient httpClient = new();

    public static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var message = update.Message;
        if (message != null && message.Text != null && message.Text.StartsWith("/start"))
        {
            string[] parts = message.Text.Split(" ");
            if (parts.Length > 1)
            {
                string startParam = parts[1];
                string chatId = message.Chat.Id.ToString();
                Console.WriteLine($"Получен параметр запуска: {startParam}");
                Console.WriteLine($"ChatId: {chatId}");

                // Отправляем запрос на сервер
                SendRequestToServerModel model = new SendRequestToServerModel(chatId, chatId);
                string serverResponse = await SendRequestToServer(model);

                // Обрабатываем ответ сервера
                if (serverResponse == "OK")
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

    private static async Task<string> SendRequestToServer(SendRequestToServerModel request)
    {
        try
        {
            string url = Environment.GetEnvironmentVariable("SERVER_URL");

            // Сериализуйте запрос в JSON
            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Отправьте POST-запрос
            var response = await httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            // Вернуть содержимое ошибки для дополнительной информации
            return "Ошибка: " + await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Ошибка при отправке запроса: {ex.Message}");
            return "Ошибка";
        }
    }
}
