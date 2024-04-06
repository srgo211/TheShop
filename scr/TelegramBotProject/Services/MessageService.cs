using Microsoft.Extensions.Logging;
using TelegramBotProject.Interfaces;
using TelegramBotProject.Interfaces.Models;

namespace TelegramBotProject.Services;

public class MessageService : BaseService, IMessageService
{
    private readonly ILogger<MessageService> logger;
    private readonly IHttpClientService httpClient;


    public MessageService(
        BotConfiguration botConfig, 
        ITelegramBotClient bot, 
        CommandSwitchController commandSwitchController,
        IHttpClientService httpClient,
        ILogger<MessageService> logger) 
        : base(botConfig, bot, commandSwitchController)
    {
        this.httpClient = httpClient;
        this.logger = logger;
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

    protected override async Task<IProduct> GetProduct(int page, int itemsPerPage)
    {
        return await httpClient.GetProduct(page, itemsPerPage);
       
    }
}