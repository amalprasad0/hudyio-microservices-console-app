namespace ChatService.Interface
{
    public interface IMessageService
    {
        Task SendMessageAsync(string toUserId, string message, string fromUserId);
    }
}
