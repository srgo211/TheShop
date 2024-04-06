using System.Collections.Concurrent;
using TelegramBotProject.Interfaces;

namespace TelegramBotProject;

public class CommandSwitchController
{
    public ConcurrentDictionary<long, ICommandStatuses> UserCommandStatuses { get; set; } = new();
}