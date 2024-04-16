namespace RealTimeNotificationSystem.Interfaces;

public interface IDataProvider<T>
{
    Task<IEnumerable<T>> FetchData();
}