namespace RealTimeNotificationSystem.Interfaces;

public interface IDataProvider<T>
{
    Task<IEnumerable<T>> FetchDataAsync();
    Task<bool> UpdateDataAsync(T data);
}