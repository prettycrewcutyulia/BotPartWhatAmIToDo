namespace ServerPartWhatAmItOdO.Models;

public struct SendRequestToServerModel
{
    public string ChatId { get; set; }
    public string Gmail { get; set; }

    public SendRequestToServerModel(string chatId, string gmail)
    {
        ChatId = chatId;
        Gmail = gmail;
    }
}