using TelegramBotProject.Interfaces.Models;

namespace TelegramBotProject.DTO;

public class Brand : Base, IBrand
{
   public string Country { get; set; }
}