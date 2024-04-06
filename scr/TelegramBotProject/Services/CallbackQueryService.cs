using TelegramBotProject.Interfaces;

namespace TelegramBotProject.Services;

public class CallbackQueryService : BaseService, ICallbackQueryService
{
    public CallbackQueryService(BotConfiguration botConfig, ITelegramBotClient bot, CommandSwitchController commandSwitchController) : base(botConfig, bot, commandSwitchController)
    {
    }

    public async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery)
    {
        await BotOnCallbackQueryReceived(callbackQuery);
    }



    internal async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
    {
        string callback = callbackQuery?.Data;
        var message = callbackQuery.Message;

        
        long userId = GetUserId(message);

        ICommandStatuses? commandStatuses =
            commandSwitchController.UserCommandStatuses.GetOrAdd(userId, new CommandStatuses());

        if (commandStatuses.Subscription == TypeStatusCommand.Wait)
        {
            Task<Message>? msg = NotificationSubscription(message);
            return;
        }


        switch (callback)
        {
            case TextComands.subscribe:
                UpStatusCommand(userId, TypeStatusCommand.Wait);
                Task<Message>? msg = NotificationSubscription(message);
                break;
            case TextComands.unsubscribe:
                await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"Вы отписались от уведомлений");
                UpStatusCommand(userId, TypeStatusCommand.Disable);
                break;
        }

    }
}