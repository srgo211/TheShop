using Microsoft.Extensions.Logging;
using TelegramBotProject.Interfaces;

namespace TelegramBotProject.Services;

public class MessageService : BaseService, IMessageService
{
    private readonly ILogger<MessageService> logger;


    public MessageService(BotConfiguration botConfig, ITelegramBotClient bot, CommandSwitchController commandSwitchController, ILogger<MessageService> logger) : base(botConfig, bot, commandSwitchController)
    {
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
            //TextComands.productСatalog => ProductСatalog(message),
            TextComands.AdminPanel => OtherMsg(message),
            TextComands.muOrders => OtherMsg(message),
            TextComands.subscribe => OtherMsg(message),
            TextComands.unsubscribe => OtherMsg(message),
            _ => OtherMsg(message)
        };
        Message sentMessage = await action;
    }
}