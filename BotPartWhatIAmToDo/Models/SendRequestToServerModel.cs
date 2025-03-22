namespace ServerPartWhatAmItOdO.Models;

public struct SendRequestToServerModel
{
    public string TgId { get; set; }
    public string Email { get; set; }

    public SendRequestToServerModel(string chatId, string gmail)
    {
        TgId = chatId;
        Email = gmail;
    }
}