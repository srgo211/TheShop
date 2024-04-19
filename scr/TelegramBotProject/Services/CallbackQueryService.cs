using TelegramBotProject.Interfaces;
using TelegramBotProject.Interfaces.Models;

namespace TelegramBotProject.Services;

public class CallbackQueryService : BaseService, ICallbackQueryService
{
    private readonly IHttpClientService httpClient;
    public CallbackQueryService(BotConfiguration botConfig, ITelegramBotClient bot, CommandSwitchController commandSwitchController, IHttpClientService httpClient) 
        : base(botConfig, bot, commandSwitchController)
    {
        this.httpClient = httpClient;
    }

    public async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery)
    {
        await BotOnCallbackQueryReceived(callbackQuery);
    }



    internal async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
    {
        string callback = callbackQuery?.Data;
        Message? message = callbackQuery.Message;

        
        long userId = GetUserId(message);

        ICommandStatuses? commandStatuses =
            commandSwitchController.UserCommandStatuses.GetOrAdd(userId, new CommandStatuses());

        if (commandStatuses.Subscription == TypeStatusCommand.Wait)
        {
            Task<Message>? msg = NotificationSubscription(message);
            return;
        }

        string[] callbacks = default;
        if (callback.Contains("|"))
        {
            callbacks = callback.Split('|');
        }

        if (callbacks is not null && callbacks.Length > 0)
        {
            callback = callbacks[0];
        }

        int page = 1;
        switch (callback)
        {
            case TextComands.subscribe:
                UpStatusCommand(userId, TypeStatusCommand.Wait);
                Task<Message>? msg = NotificationSubscription(message);
                return;
            case TextComands.unsubscribe:
                UpStatusCommand(userId, TypeStatusCommand.Disable);
                await bot.AnswerCallbackQueryAsync(callbackQuery.Id,
                    $"Вы отписались от уведомлений", 
                    cacheTime:100,
                    showAlert:true);
               await ProductСatalog(message, page, 1);
               return;

            case TextComands.nextProductBtn:
                int.TryParse(callbacks.Last(), out  page);
                await ProductСatalogEdite(callbackQuery, page, 1);
                return;

            case TextComands.prevProductBtn:
                int.TryParse(callbacks.Last(), out page);
                await ProductСatalogEdite(callbackQuery, page, 1);
                return;


            default: await bot.AnswerCallbackQueryAsync(callbackQuery.Id,callback,cacheTime: 2000, showAlert: false);
            break;

        }
        
       

    }


    protected override async Task<IProduct> GetProduct(int page, int itemsPerPage)
    {
        return await httpClient.GetProduct(page, itemsPerPage);

    }

    protected override async Task UpUserAsync(long userId, string email)
    {
        bool check = await httpClient.UpUser(userId, email);

    }


    async Task edite(CallbackQuery callbackQuery)
    {
        // ID чата и ID сообщения, которое нужно изменить
        long chatId = callbackQuery.Message.Chat.Id;
        int messageId = callbackQuery.Message.MessageId;

        // Новый текст сообщения и набор кнопок
        string newText = "Измененный текст сообщения";
        InlineKeyboardMarkup newInlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("New button", "new_action")
        });


        // Изменение текста сообщения
        await bot.EditMessageTextAsync(
            chatId: chatId,
            messageId: messageId,
            text: newText,
            replyMarkup: newInlineKeyboard,
            parseMode:ParseMode.Html
        );
        await bot.AnswerCallbackQueryAsync(callbackQuery.Id);

    }
}