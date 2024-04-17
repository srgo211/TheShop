﻿using Telegram.Bot.Types;
using TelegramBotProject.BusinessLogic;
using TelegramBotProject.DTO;
using TelegramBotProject.Interfaces;
using TelegramBotProject.Interfaces.Models;

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
        long userId = GetUserId(message);

        commandSwitchController.UserCommandStatuses.TryGetValue(userId, out ICommandStatuses? commandStatus);

      
        bool chek = DataValidator.IsValidEmail(message.Text);
       

        if (commandStatus.Subscription == TypeStatusCommand.Wait && !chek)
        {
            return await SendMessage(message.Chat.Id, "Введите email");
        }

        if (commandStatus.Subscription == TypeStatusCommand.Wait && chek)
        {

            string email = message.Text;
            //TODO подписка
            UpStatusCommand(userId, TypeStatusCommand.Enable);
            UpUser(userId, email);

            // Удаляем сообщение пользователя
            ///await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            
            string text = $"Вы подписались на уведомления: <u>{email}</u>";
            await SendMessage(message.Chat.Id, text);

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

   

   

    protected async Task<Message> MenuStore(Message message)
    {
        

        string text = $"Пожалуйста ознакомьтесь с меню магазина";
        //f (!String.IsNullOrWhiteSpace(addText)) text = $"{addText}\n\n{text}";

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
        
        string text = "Нам пока не нужны эти данные! Спасибо!\n";

       
        return await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: text, parseMode: ParseMode.Html,
            disableWebPagePreview: false);
    }


    protected async Task<Message> ProductСatalog(Message message, int page, int itemsPerPage)
    {

        IProduct product = await GetProduct(page, itemsPerPage);

        string url = $"{botConfig.HostFilesAddress}/img/{product.Images.FirstOrDefault()?.Name}";
        string link = $"<a href=\"{url}\">link</a>";

        int productId                 = product.Id;
        string productName            = product.Name;
        decimal productPrice       = product.Price;
        int productStockQuantity   = product.StockQuantity;
        string productDescription  = product.Description;
        string brandName              = product.Brand.Name;
        string categorieName          = product.Categorie.Name;

        string text = 
               $"<b>Просмотр товара в категории: <u>{categorieName}</u></b>\n\n" +
               $"<b>{productName}</b> | <i>{brandName}</i>\n\n" +
               $"<b>Описание:</b><code>{productDescription}</code>\n\n" +
               $"<b>Цена:</b> {productPrice} руб. | {productStockQuantity} шт<a href=\"{url}\">.</a>";


        int pagePrev = page-1 <=0 ? 1 : page-1;
        int pageNext = page + 1;
        InlineKeyboardMarkup keyboard = new(
            new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextComands.prevProductBtn, $"{TextComands.prevProductBtn}|{pagePrev}"),
                    InlineKeyboardButton.WithCallbackData($"ID: {productId}", productId.ToString()),
                    InlineKeyboardButton.WithCallbackData(TextComands.nextProductBtn, $"{TextComands.nextProductBtn}|{pageNext}"),
                },

                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextComands.backBtn, TextComands.backBtn),
                    InlineKeyboardButton.WithCallbackData(TextComands.addOrderBtn, TextComands.addOrderBtn),
                },

            });



        return await bot.SendTextMessageAsync(chatId: message.Chat.Id, 
            text: text, 
            parseMode: ParseMode.Html,
            replyMarkup:keyboard,
            disableWebPagePreview: false
            );



        return message;
    }

    protected async Task<Message> ProductСatalogEdite(CallbackQuery callbackQuery, int page, int itemsPerPage)
    {

        // ID чата и ID сообщения, которое нужно изменить
        long chatId   = callbackQuery.Message.Chat.Id;
        int messageId = callbackQuery.Message.MessageId;


        IProduct product = await GetProduct(page, itemsPerPage);

        string url = $"{botConfig.HostFilesAddress}/img/{product.Images.FirstOrDefault()?.Name}";
        string link = $"<a href=\"{url}\">link</a>";

        int productId = product.Id;
        string productName = product.Name;
        decimal productPrice = product.Price;
        int productStockQuantity = product.StockQuantity;
        string productDescription = product.Description;
        string brandName = product.Brand.Name;
        string categorieName = product.Categorie.Name;

        string text =
               $"<b>Просмотр товара в категории: <u>{categorieName}</u></b>\n\n" +
               $"<b>{productName}</b> | <i>{brandName}</i>\n\n" +
               $"<b>Описание: </b><code>{productDescription}</code>\n\n" +
               $"<b>Цена:</b> {productPrice} руб. | {productStockQuantity} шт<a href=\"{url}\">.</a>";


        int pagePrev = page - 1 <= 0 ? 1 : page - 1;
        int pageNext = page + 1;
        InlineKeyboardMarkup keyboard = new(
            new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextComands.prevProductBtn, $"{TextComands.prevProductBtn}|{pagePrev}"),
                    InlineKeyboardButton.WithCallbackData($"ID: {productId}", productId.ToString()),
                    InlineKeyboardButton.WithCallbackData(TextComands.nextProductBtn, $"{TextComands.nextProductBtn}|{pageNext}"),
                },

                new[]
                {
                    InlineKeyboardButton.WithCallbackData(TextComands.backBtn, TextComands.backBtn),
                    InlineKeyboardButton.WithCallbackData(TextComands.addOrderBtn, TextComands.addOrderBtn),
                },

            }
            
            );



        // Изменение текста сообщения
        Message message = await bot.EditMessageTextAsync(
            chatId: chatId,
            messageId: messageId,
            text: text,
            replyMarkup: keyboard,
            parseMode:ParseMode.Html
            
        );
        await bot.AnswerCallbackQueryAsync(callbackQuery.Id);

        return message;
    }


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
        if (commandSwitchController.UserCommandStatuses.TryGetValue(userId, out ICommandStatuses? value))
        {
            CommandStatuses newModel = new CommandStatuses(value.Subscription = statusCommand);
            bool updateSuccess = commandSwitchController.UserCommandStatuses.TryUpdate(userId, newModel, value);
        }
    }

    protected virtual async Task<IProduct> GetProduct(int page, int itemsPerPage)
    {
        return new Product();
    }

    protected virtual async Task UpUser(long userId, string email)
    {
    }

}