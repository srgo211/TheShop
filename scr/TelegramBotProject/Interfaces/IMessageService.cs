namespace TelegramBotProject.Interfaces;

public interface IMessageService
{
    Task HandleMessageAsync(Message message);
    Task HandleEditedMessageAsync(Message message);
}

public interface ICallbackQueryService
{
    Task HandleCallbackQueryAsync(CallbackQuery callbackQuery);
}