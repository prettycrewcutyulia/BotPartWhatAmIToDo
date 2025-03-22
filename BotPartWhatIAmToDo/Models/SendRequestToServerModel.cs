namespace ServerPartWhatAmItOdO.Models;

public struct SendRequestToServerModel
{
    public long TgId { get; set; }
    public string Email { get; set; }

    public SendRequestToServerModel(long chatId, string gmail)
    {
        TgId = chatId;
        Email = gmail;
    }
}