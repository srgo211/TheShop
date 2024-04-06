using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using TelegramBotProject.Interfaces;

namespace TelegramBotProject.BusinessLogic;

public class CallbackQuerysService :  Base
{
    public CallbackQuerysService(ITelegramBotClient bot, IHttpClientService httpClient, ILogger<HandleUpdateService> logger, IOptions<BotConfiguration> botConfig, CommandSwitchController commandSwitchController, CallbackQuerysService callbackQuerysService) 
        : base(bot, httpClient, logger, botConfig, commandSwitchController, callbackQuerysService)
    {
    }

    internal async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
    {
        string callback = callbackQuery?.Data;
        var message = callbackQuery.Message;

        long userId = message.Contact?.UserId ?? default;
        ICommandStatuses? commandStatuses = commandSwitchController.UserCommandStatuses.GetOrAdd(userId, new CommandStatuses());

        if (commandStatuses.Subscription == TypeStatusCommand.Wait)
        {
            Task<Message>? msg = NotificationSubscription(message);
            return;
        }


        switch (callback)
        {
            case TextComands.subscribe: break;
            case TextComands.unsubscribe: break;
        }

        var loginUrl = new LoginUrl
        {
            Url = "https://yourdomain.com/login?bot_id=YOUR_BOT_ID&request_access=email", // Your login URL
            ForwardText = "Login to share your email",
            BotUsername = "your_bot_username", // Without @
            RequestWriteAccess = true
        };

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithLoginUrl("Share Email", loginUrl)
        });


        // Идентификатор чата
        var chatId = callbackQuery.Message?.Chat.Id;

        // Идентификатор сообщения
        var messageId = callbackQuery.Message.MessageId;



        // Отправка уведомления пользователю, который совершил CallbackQuery
        await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"Вы выбрали: {callback}");



        await bot.SendTextMessageAsync(
            chatId: chatId,
            text: "Please share your email with us:"
        );
    }

    
}