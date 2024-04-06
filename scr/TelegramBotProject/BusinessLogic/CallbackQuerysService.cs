using TelegramBotProject.Interfaces;

namespace TelegramBotProject.BusinessLogic;

public class CallbackQuerysService
{
    protected readonly CallbackQuerysService callbackQuerysService;
    protected readonly CommandSwitchController commandSwitchController;
   

    public CallbackQuerysService(CallbackQuerysService callbackQuerysService)
    {
        this.callbackQuerysService = callbackQuerysService;
       
    }

    internal async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
    {
        string callback = callbackQuery?.Data;
        var message = callbackQuery.Message;

        long userId = message.Contact?.UserId ?? default;
        ICommandStatuses? commandStatuses = commandSwitchController.UserCommandStatuses.GetOrAdd(userId, new CommandStatuses());

        if (commandStatuses.Subscription == TypeStatusCommand.Wait)
        {
            //Task<Message>? msg = NotificationSubscription(message);
            return;
        }


        switch (callback)
        {
            case TextComands.subscribe:
               // Task<Message>? msg = NotificationSubscription(message);

                break;
            case TextComands.unsubscribe:
                //await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"Вы отписались от уведомлений");
                if (commandSwitchController.UserCommandStatuses.TryGetValue(userId, out var value))
                {
                    var newModel = new CommandStatuses(value.Subscription = TypeStatusCommand.Disable);
                    bool updateSuccess = commandSwitchController.UserCommandStatuses.TryUpdate(userId, newModel, value);
                }


                break;
        }

       

        


    }

    
}