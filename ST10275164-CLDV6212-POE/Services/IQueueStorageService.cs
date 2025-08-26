namespace ST10275164_CLDV6212_POE.Services
{
    public interface IQueueStorageService
    {
        Task SendMessageAsync(string message);
    }
}
