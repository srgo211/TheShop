using Microsoft.Extensions.Options;
using TelegramBotProject.BusinessLogic;
using TelegramBotProject.Interfaces;

namespace TelegramBotProject;

public class Base
{
    internal readonly BotConfiguration botConfig;
    internal readonly ITelegramBotClient bot;
    internal readonly IHttpClientService httpClient;
    internal readonly CommandSwitchController commandSwitchController;
    

    public Base(
        ITelegramBotClient bot, 
        IHttpClientService httpClient,
        IOptions<BotConfiguration> botConfig,
        CommandSwitchController commandSwitchController
        )
    {
        this.bot = bot;
        this.httpClient = httpClient;
        this.botConfig = botConfig.Value;
        this.commandSwitchController = commandSwitchController;
       

    }


    protected async Task<Message>? NotificationSubscription(Message message)
    {
        long userId = message.Contact?.UserId ?? default;
        bool chek = DataValidator.IsValidEmail(message.Text);
        if (chek)
        {
            if (commandSwitchController.UserCommandStatuses.TryGetValue(userId, out var value))
            {
                var newModel = new CommandStatuses(value.Subscription = TypeStatusCommand.Enable);
                bool updateSuccess = commandSwitchController.UserCommandStatuses.TryUpdate(userId, newModel, value);

                return await MenuStore(message);
            }
            
        }
        return await SendMessage(message.Chat.Id, "Не верно введен email, попробуйте еще раз");

    }


    protected async Task<Message> MenuStore(Message message)
    {


        string text = $"Пожалуйста ознакомьтесь с меню магазина";
        

        ReplyKeyboardMarkup keyboard = new(
            new[]
            {
                new KeyboardButton[] { TextComands.productСatalog, TextComands.muOrders },
                new KeyboardButton[] {TextComands.AdminPanel},
            })
        {
            ResizeKeyboard = true
        };




        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
            text: text,
            parseMode: ParseMode.Html,
            replyMarkup: keyboard,
            disableWebPagePreview: false
        );
    }


    protected async Task<Message> SendMessage(long chatId, string text)
    {
        
        return await bot.SendTextMessageAsync(chatId: chatId,
            text: text,
            parseMode: ParseMode.Html,
            disableWebPagePreview: false
        );
    }
}