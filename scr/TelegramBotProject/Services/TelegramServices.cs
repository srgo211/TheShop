using TelegramBotProject.BusinessLogic;

namespace TelegramBotProject.Services;

public class TelegramServices
{
    public HandleUpdateService HandleUpdateService { get; set; }
    public CallbackQuerysService CallbackQuerysService { get; set; }
}