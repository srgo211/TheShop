using Microsoft.Extensions.Logging;
using TelegramBotProject.Interfaces;
using TelegramBotProject.Interfaces.Models;

namespace TelegramBotProject.Services;

public class MessageService : BaseService, IMessageService
{
    private readonly ILogger<MessageService> logger;
    private readonly IHttpClientService httpClient;
    public MessageService(BotConfiguration botConfig, ITelegramBotClient bot, CommandSwitchController commandSwitchController, IHttpClientService httpClient, ILogger<MessageService> logger) 
        : base(botConfig, bot, commandSwitchController)
    {
        this.logger = logger;
        this.httpClient = httpClient;
    }

    public Task HandleEditedMessageAsync(Message message)
    {
        throw new NotImplementedException();
    }

    public async Task HandleMessageAsync(Message message)
    {
        logger.LogInformation("Receive message type: {messageType}", message.Type);
        if (message.Type != MessageType.Text) return;

        long userId = GetUserId(message);

        ICommandStatuses? commandStatuses =
            commandSwitchController.UserCommandStatuses.GetOrAdd(userId, new CommandStatuses());

        if (commandStatuses.Subscription == TypeStatusCommand.Wait)
        {
            Task<Message>? msg = NotificationSubscription(message);
            return;
        }



        Task<Message>? action = message.Text?.Trim() switch
        {
            TextComands.start => Start(message),
            TextComands.menu => MenuStore(message),
            TextComands.productСatalog => ProductСatalog(message, 1, 1),
            TextComands.AdminPanel => OtherMsg(message),
            TextComands.muOrders => OtherMsg(message),
            TextComands.subscribe => OtherMsg(message),
            TextComands.unsubscribe => OtherMsg(message),
            _ => OtherMsg(message)
        };
        Message sentMessage = await action;
    }


    protected async Task<Message> Start(Message message)
    {
        long userId = GetUserId(message);
        await RegisterUserAsync(userId);

        if (commandSwitchController.UserCommandStatuses.ContainsKey(userId))
        {
            CommandStatuses newCommandStatus = new CommandStatuses();
            commandSwitchController.UserCommandStatuses.AddOrUpdate(userId, newCommandStatus, (key, oldValue) => newCommandStatus);


        }
        else commandSwitchController.UserCommandStatuses.TryAdd(userId, new CommandStatuses());

        ICommandStatuses? commandStatuses =
            commandSwitchController.UserCommandStatuses.GetOrAdd(userId, new CommandStatuses());


        string url = $"{botConfig.HostFilesAddress}/img/logo.jpg";
        string text = $"Добро пожаловать в интернет-магазин<a href=\"{url}\">!</a>";


        InlineKeyboardMarkup keyboard = new(
            new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextComands.subscribe, TextComands.subscribe),
                    InlineKeyboardButton.WithCallbackData(TextComands.unsubscribe, TextComands.unsubscribe),

                },
            });


        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
            text: text,
            parseMode: ParseMode.Html,
            replyMarkup: keyboard,
            disableWebPagePreview: false
        );
    }


    protected override async Task<IProduct> GetProduct(int page, int itemsPerPage)
    {
        return await httpClient.GetProduct(page, itemsPerPage);
       
    }


    private async Task RegisterUserAsync(long userId)
    {
        var checkUser = await httpClient.CheckUser(userId);
        if (!checkUser) await httpClient.AddUser(userId);
    }
}