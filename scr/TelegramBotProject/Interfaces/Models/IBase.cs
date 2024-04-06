namespace TelegramBotProject.Interfaces.Models;

public interface IBase
{
    int Id { get; set; }

    /// <summary>Наименование</summary>
    string Name { get; set; }
}