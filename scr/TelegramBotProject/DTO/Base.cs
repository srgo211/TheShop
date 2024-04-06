using TelegramBotProject.Interfaces.Models;

namespace TelegramBotProject.DTO;

public class Base : IBase
{
    public int Id { get; set; }
    public string Name { get; set; }
}