namespace TelegramBotProject.Interfaces;


public enum TypeStatusCommand
{
    None    = 0,
    Enable  = 2,
    Disable = 4,
    Wait    = 8,
}

public interface ICommandStatuses
{
    /// <summary>статус о подписке</summary>
    public TypeStatusCommand Subscription { get; set; }
}