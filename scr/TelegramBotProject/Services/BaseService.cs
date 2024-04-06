using Microsoft.Extensions.Logging;
using System.Net.Http;
using Telegram.Bot.Types;
using TelegramBotProject.BusinessLogic;
using TelegramBotProject.Interfaces;

namespace TelegramBotProject.Services;

public class BaseService
{
    protected readonly CommandSwitchController commandSwitchController;
    protected readonly ITelegramBotClient bot;
    protected readonly BotConfiguration botConfig;

    public BaseService(BotConfiguration botConfig,ITelegramBotClient bot, CommandSwitchController commandSwitchController)
    {
        this.botConfig = botConfig;
        this.bot = bot;
        this.commandSwitchController = commandSwitchController;
    }

    protected async Task<Message>? NotificationSubscription(Message message)
    {
        var userId = GetUserId(message);

        commandSwitchController.UserCommandStatuses.TryGetValue(userId, out var commandStatus);

      
        bool chek = DataValidator.IsValidEmail(message.Text);
       

        if (commandStatus.Subscription == TypeStatusCommand.Wait && !chek)
        {
            return await SendMessage(message.Chat.Id, "Введите email");
        }

        if (commandStatus.Subscription == TypeStatusCommand.Wait && chek)
        {
            UpStatusCommand(userId, TypeStatusCommand.Enable);
            return await MenuStore(message);
        }


        return message;
    }

    protected static long GetUserId(Message message)
    {
        //long userId = message.From?.Id ?? default;
        long userId = message.Chat?.Id ?? default;
        return userId;
    }

    protected async Task<Message> Start(Message message)
    {
        var userId = GetUserId(message);

        if (commandSwitchController.UserCommandStatuses.ContainsKey(userId))
        {
            var newCommandStatus = new CommandStatuses();
            commandSwitchController.UserCommandStatuses.AddOrUpdate(userId, newCommandStatus, (key, oldValue) => newCommandStatus);


        }
        else commandSwitchController.UserCommandStatuses.TryAdd(userId, new CommandStatuses());

        ICommandStatuses? commandStatuses =
            commandSwitchController.UserCommandStatuses.GetOrAdd(userId, new CommandStatuses());


        string url = $"{botConfig.HostFilesAddress}/img/logo.jpg";
        string text = $"Добро пожаловать в интернет-магазин<a href=\"{url}\">!</a>";


        InlineKeyboardMarkup keyboard = new(
            new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextComands.subscribe, TextComands.subscribe),
                    InlineKeyboardButton.WithCallbackData(TextComands.unsubscribe, TextComands.unsubscribe),

                },
            });


        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
            text: text,
            parseMode: ParseMode.Html,
            replyMarkup: keyboard,
            disableWebPagePreview: false
        );
    }

    protected async Task<Message> MenuStore(Message message)
    {


        string text = $"Пожалуйста ознакомьтесь с меню магазина";


        ReplyKeyboardMarkup keyboard = new(
            new[]
            {
                new KeyboardButton[] { TextComands.productСatalog, TextComands.muOrders },
                new KeyboardButton[] {TextComands.AdminPanel},
            })
        {
            ResizeKeyboard = true
        };




        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
            text: text,
            parseMode: ParseMode.Html,
            replyMarkup: keyboard,
            disableWebPagePreview: false
        );
    }

    protected async Task<Message> SendMessage(long chatId, string text)
    {

        return await bot.SendTextMessageAsync(chatId: chatId,
            text: text,
            parseMode: ParseMode.Html,
            disableWebPagePreview: false
        );
    }

    protected async Task<Message> OtherMsg(Message message)
    {
        //string url = "https://disk.yandex.ru/i/9hfkr-gp4YWcKQ";
        string url = "https://0ac8-31-171-195-46.ngrok-free.app/img/1.jpg";
        //string url = "http://localhost:8080/img/1.jpg";
        string link = $"<a href=\"{url}\">link</a>";
        string text = "Нам пока не нужны эти данные! Спасибо!\n" + link;

        text = "<b>Просмотр товара в категории: <u>Подарки, книги, игры</u></b>\n\n" +
               "<s>strikethrough</s>" +
               "Фанты-флирт №8 Бутылочка\n\n" +
               "<b>Описание:</b><code>Фанты «Бутылочка» — это игра-флирт для компании до десяти человек. Здесь не надо крутить бутылочку, зато можно здорово оторваться на горячей вечеринке или получить массу новых ощущений в отпуске или в дороге.</code>\n" +
               $"<b>Цена:</b> 620.00 | шт<a href=\"{url}\">.</a>";

        return await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: text, parseMode: ParseMode.Html,
            disableWebPagePreview: false);
    }


    //private async Task<Message> ProductСatalog(Message message)
    //{
    //    string url = "http://localhost:5277/products/paged?page=1&itemsPerPage=1";
    //    var response = await httpClient.GetAsync(url);
    //    if (response.IsSuccessStatusCode)
    //    {

    //        string content = await response.Content.ReadAsStringAsync();
    //        logger.LogInformation(content);
    //    }

    //    return message;
    //}

    protected async Task<Message> SendProduct(Message message)
    {
        //string url = "https://disk.yandex.ru/i/9hfkr-gp4YWcKQ";
        string url = "https://0ac8-31-171-195-46.ngrok-free.app/img/1.jpg";
        //string url = "http://localhost:8080/img/1.jpg";
        string link = $"<a href=\"{url}\">link</a>";
        string text = "Нам пока не нужны эти данные! Спасибо!\n" + link;

        text = "<b>Просмотр товара в категории: <u>Подарки, книги, игры</u></b>\n\n" +
               "<s>strikethrough</s>" +
               "Фанты-флирт №8 Бутылочка\n\n" +
               "<b>Описание:</b><code>Фанты «Бутылочка» — это игра-флирт для компании до десяти человек. Здесь не надо крутить бутылочку, зато можно здорово оторваться на горячей вечеринке или получить массу новых ощущений в отпуске или в дороге.</code>\n" +
               $"<b>Цена:</b> 620.00 | шт<a href=\"{url}\">.</a>";

        return await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: text, parseMode: ParseMode.Html,
            disableWebPagePreview: false);
    }

    protected void UpStatusCommand(long userId, TypeStatusCommand statusCommand)
    {
        if (commandSwitchController.UserCommandStatuses.TryGetValue(userId, out var value))
        {
            var newModel = new CommandStatuses(value.Subscription = statusCommand);
            bool updateSuccess = commandSwitchController.UserCommandStatuses.TryUpdate(userId, newModel, value);
        }
    }
}