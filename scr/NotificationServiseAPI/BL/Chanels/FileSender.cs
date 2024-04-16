using NotificationServiseAPI.Interfaces;
using SharedDomainModels;
using System.Text.Json;

namespace NotificationServiseAPI.BL.Chanels;

public class FileSender : IMessageSender
{
    private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
    private readonly string filePath;

    public FileSender(string filePath)
    {
        this.filePath = filePath;
    }

    public async Task SendAsync(Notification notification)
    {
        
        string json = JsonSerializer.Serialize(notification);

        await semaphore.WaitAsync();

        try
        {
           
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                await writer.WriteLineAsync(json);
                await writer.FlushAsync();
            }
        }
        finally
        {
            
            semaphore.Release();
        }
    }
}