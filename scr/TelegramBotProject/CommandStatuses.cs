using TelegramBotProject.Interfaces;

namespace TelegramBotProject;

public class CommandStatuses : ICommandStatuses
{
    public CommandStatuses()
    {
        
    }

    public CommandStatuses(TypeStatusCommand subscription)
    {
        this.Subscription = subscription;
    }

    public TypeStatusCommand Subscription { get; set; }
}