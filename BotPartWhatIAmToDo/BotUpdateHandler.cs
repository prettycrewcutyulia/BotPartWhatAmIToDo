using Telegram.Bot;
using Telegram.Bot.Types;

public class BotUpdateHandler
{
    private static readonly HttpClient httpClient = new HttpClient();

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
                string serverResponse = await SendRequestToServer(startParam);

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

    private static async Task<string> SendRequestToServer(string parameter)
    {

        return "OK";
        try
        {
            // Пример URL, замените на ваш реальный URL и добавьте нужные параметры
            string url = $"https://example.com/api/verify?param={parameter}";
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return "Ошибка";
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Ошибка при отправке запроса: {ex.Message}");
            return "Ошибка";
        }
    }
}
