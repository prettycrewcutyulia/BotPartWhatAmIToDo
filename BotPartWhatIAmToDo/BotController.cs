using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
[ApiController]
[Route("api/[controller]")]
public class BotController : ControllerBase
{
    private readonly ITelegramBotClient _botClient;

    public BotController()
    {
        // Загрузите переменные окружения из .env файла
        DotNetEnv.Env.Load();

        // Получите токен из переменных окружения
        string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException("TELEGRAM_BOT_TOKEN не может быть пустым");
        }

        _botClient = new TelegramBotClient(token);
    }

    
    /// <summary>
    /// Ручка для отправки напоминания-сообщения с сервера
    /// </summary>
    /// <param name="request">Кому и что отправить</param>
    /// <returns>Успешность отправки</returns>
    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest? request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty.");
        }

        try
        {
            await _botClient.SendMessage(request.ChatId, request.Message);
            return Ok("Message sent successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error sending message: {ex.Message}");
        }
    }
}