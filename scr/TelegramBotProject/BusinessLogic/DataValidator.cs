using System.Text.RegularExpressions;

namespace TelegramBotProject.BusinessLogic;

public class DataValidator
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}