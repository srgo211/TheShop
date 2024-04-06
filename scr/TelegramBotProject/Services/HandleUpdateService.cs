using Microsoft.Extensions.Options;
using TelegramBotProject.Interfaces;

namespace TelegramBotProject.Services;

public class HandleUpdateService : BaseService 
{
    private readonly ILogger<HandleUpdateService> _logger;
    private readonly ICommandHandler _commandHandler;
    private readonly ICallbackQueryService _callbackQueryService;
    private readonly IMessageService _messageService;


    public HandleUpdateService(BotConfiguration botConfig, ITelegramBotClient bot, CommandSwitchController commandSwitchController, ILogger<HandleUpdateService> logger, ICommandHandler commandHandler, ICallbackQueryService callbackQueryService, IMessageService messageService) : base(botConfig, bot, commandSwitchController)
    {
        _logger = logger;
        _commandHandler = commandHandler;
        _callbackQueryService = callbackQueryService;
        _messageService = messageService;
    }

    public async Task HandleUpdateAsync(Update update)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await _messageService.HandleMessageAsync(update.Message);
                    break;
                case UpdateType.EditedMessage:
                    await _messageService.HandleEditedMessageAsync(update.EditedMessage);
                    break;
                case UpdateType.CallbackQuery:
                    await _callbackQueryService.HandleCallbackQueryAsync(update.CallbackQuery);
                    break;
                default:
                    _logger.LogInformation($"Unhandled update type: {update.Type}");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling update.");
        }
    }

}