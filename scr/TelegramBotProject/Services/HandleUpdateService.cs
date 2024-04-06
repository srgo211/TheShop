using Microsoft.Extensions.Options;
using System.Text;
using TelegramBotProject.BusinessLogic;
using TelegramBotProject.Interfaces;

namespace TelegramBotProject.Services;

public class HandleUpdateService : Base
{
    public HandleUpdateService(ITelegramBotClient bot, IHttpClientService httpClient, ILogger<HandleUpdateService> logger, IOptions<BotConfiguration> botConfig, CommandSwitchController commandSwitchController, CallbackQuerysService callbackQuerysService) : base(bot, httpClient, logger, botConfig, commandSwitchController, callbackQuerysService)
    {
    }

    public async Task UpdateMessageAsync(Update update)
    {
        var handler = update.Type switch
        {
            // UpdateType.Unknown:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            UpdateType.Message            => BotOnMessageReceived(update.Message!),
            UpdateType.EditedMessage      => BotOnMessageReceived(update.EditedMessage!),
            UpdateType.CallbackQuery      => callbackQuerysService.BotOnCallbackQueryReceived(update.CallbackQuery!),//BotOnCallbackQueryReceived(update.CallbackQuery!),
            UpdateType.InlineQuery        => BotOnInlineQueryReceived(update.InlineQuery!),
            UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult!),
            _ => UnknownUpdateHandlerAsync(update)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception);
        }
    }

    private async Task BotOnMessageReceived(Message message)
    {
        logger.LogInformation("Receive message type: {messageType}", message.Type);
        if (message.Type != MessageType.Text)
            return;

        long userId = message.Contact?.UserId ?? default;
        ICommandStatuses? commandStatuses = commandSwitchController.UserCommandStatuses.GetOrAdd(userId, new CommandStatuses());

        if (commandStatuses.Subscription == TypeStatusCommand.Wait)
        {
            Task<Message>? msg = NotificationSubscription(message);
            return;
        }



        Task<Message>? action = message.Text?.Trim() switch
        {
            TextComands.menu => MenuStore(message),
            TextComands.productСatalog => ProductСatalog(message),
            TextComands.AdminPanel => OtherMsg(message),
            TextComands.muOrders => OtherMsg(message),
            TextComands.subscribe => OtherMsg(message),
            TextComands.unsubscribe => OtherMsg(message),
            "/inline" => SendInlineKeyboard(bot, message),
            "/keyboard" => SendReplyKeyboard(bot, message),
            "/remove" => RemoveKeyboard(bot, message),
            "/photo" => SendFile(bot, message),
            "/request" => RequestContactAndLocation(bot, message),
            _ => OtherMsg(message)
        };
        Message sentMessage = await action;
        logger.LogInformation("The message was sent with id: {sentMessageId}", sentMessage.MessageId);

    
        async Task<Message> SendInlineKeyboard(ITelegramBotClient bot, Message message)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            // Simulate longer running task
            await Task.Delay(500);

            const string filePath = @"D:\Test\Images\1.jpg";
            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            string fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

            string text = @"Просмотр товара в категории: Подарки, книги, игры
                            Набор ""Geisha's Secret. Экзотический зеленый чай"" 5 предмето";

          


            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(TextComands.prevProductBtn, TextComands.prevProductBtn),
                        InlineKeyboardButton.WithCallbackData("3 из 10", ""),
                        InlineKeyboardButton.WithCallbackData(TextComands.nextProductBtn, TextComands.nextProductBtn),
                        
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(TextComands.backBtn,TextComands.backBtn),
                        InlineKeyboardButton.WithCallbackData(TextComands.addOrderBtn, TextComands.addOrderBtn),
                    },
                });


            string img = @"D:\Test\Images\1.jpg";
            await SendPhotoAsync(message.Chat.Id, img);
            return message;
            //return await SendPhotoAsync(message, img);



            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: "Choose",
                                                  replyMarkup: inlineKeyboard);
        }

        static async Task<Message> SendReplyKeyboard(ITelegramBotClient bot, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                        new KeyboardButton[] { "1.1", "1.2" },
                        new KeyboardButton[] { "2.1", "2.2" },
                })
            {
                ResizeKeyboard = true
            };

            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: "Choose",
                                                  replyMarkup: replyKeyboardMarkup);
        }

        static async Task<Message> RemoveKeyboard(ITelegramBotClient bot, Message message)
        {
            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: "Removing keyboard",
                                                  replyMarkup: new ReplyKeyboardRemove());
        }

        static async Task<Message> SendFile(ITelegramBotClient bot, Message message)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

            const string filePath = @"Files/tux.png";
            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            string fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

            return await bot.SendPhotoAsync(chatId: message.Chat.Id,
                                            photo: new InputFileStream(fileStream, fileName),
                                            
                                            caption: "Nice Picture");
        }

        static async Task<Message> RequestContactAndLocation(ITelegramBotClient bot, Message message)
        {
            ReplyKeyboardMarkup RequestReplyKeyboard = new(
                new[]
                {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                });

            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: "Who or Where are you?",
                                                  replyMarkup: RequestReplyKeyboard);
        }

        static async Task<Message> Usage(ITelegramBotClient bot, Message message)
        {
            const string usage = "Usage:\n" +
                                 "/inline   - send inline keyboard\n" +
                                 "/keyboard - send custom keyboard\n" +
                                 "/remove   - remove custom keyboard\n" +
                                 "/photo    - send a photo\n" +
                                 "/request  - request location or contact";

            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: usage,
                                                  replyMarkup: new ReplyKeyboardRemove());
        }
    }

    private async Task<Message>? NotificationSubscription(Message message)
    {
        throw new NotImplementedException();
    }

    private async Task<Message> ProductСatalog(Message message)
    {
        string url = "http://localhost:5277/products/paged?page=1&itemsPerPage=1";
        var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            
            string content = await response.Content.ReadAsStringAsync();
            logger.LogInformation(content);
        }

        return message;
    }

    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
    {
        string callback = callbackQuery?.Data;


        switch (callback)
        {
            case TextComands.subscribe:   break;
            case TextComands.unsubscribe: break;
        }

        var loginUrl = new LoginUrl
        {
            Url = "https://yourdomain.com/login?bot_id=YOUR_BOT_ID&request_access=email", // Your login URL
            ForwardText = "Login to share your email",
            BotUsername = "your_bot_username", // Without @
            RequestWriteAccess = true
        };

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithLoginUrl("Share Email", loginUrl)
        });


        // Идентификатор чата
        var chatId = callbackQuery.Message?.Chat.Id;

        // Идентификатор сообщения
        var messageId = callbackQuery.Message.MessageId;

      
        
        // Отправка уведомления пользователю, который совершил CallbackQuery
        await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"Вы выбрали: {callback}");



        await bot.SendTextMessageAsync(
            chatId: chatId,
            text: "Please share your email with us:"
            );
    }

    #region Inline Mode

    private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery)
    {
        logger.LogInformation("Received inline query from: {inlineQueryFromId}", inlineQuery.From.Id);

        InlineQueryResult[] results = {
            // displayed result
            new InlineQueryResultArticle(
                id: "3",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent(
                    "hello"
                )
            )
        };

        await bot.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
                                                results: results,
                                                isPersonal: true,
                                                cacheTime: 0);
    }

    private Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)
    {
        logger.LogInformation("Received inline result: {chosenInlineResultId}", chosenInlineResult.ResultId);
        return Task.CompletedTask;
    }

    #endregion

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {updateType}", update.Type);
        return Task.CompletedTask;
    }

    public Task HandleErrorAsync(Exception exception)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        return Task.CompletedTask;
    }



    


    async Task<Message> OtherMsg(Message message)
    {
        //string url = "https://disk.yandex.ru/i/9hfkr-gp4YWcKQ";
        string url = "https://0ac8-31-171-195-46.ngrok-free.app/img/1.jpg";
        //string url = "http://localhost:8080/img/1.jpg";
        string link = $"<a href=\"{url}\">link</a>";
        string text = "Нам пока не нужны эти данные! Спасибо!\n" + link;

        text = "<b>Просмотр товара в категории: <u>Подарки, книги, игры</u></b>\n\n" +
               "<s>strikethrough</s>"+
               "Фанты-флирт №8 Бутылочка\n\n" +
               "<b>Описание:</b><code>Фанты «Бутылочка» — это игра-флирт для компании до десяти человек. Здесь не надо крутить бутылочку, зато можно здорово оторваться на горячей вечеринке или получить массу новых ощущений в отпуске или в дороге.</code>\n" +
               $"<b>Цена:</b> 620.00 | шт<a href=\"{url}\">.</a>"; 

        return await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: text, parseMode: ParseMode.Html,
            disableWebPagePreview:false);
    }

    async Task<Message> SendPhotoAsync(Message message, string photoPath)
    {


        IReplyMarkup inlineKeyboard = new InlineKeyboardMarkup(
            new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(TextComands.prevProductBtn, TextComands.prevProductBtn),
                    InlineKeyboardButton.WithCallbackData("3 из 10", ""),
                    InlineKeyboardButton.WithCallbackData(TextComands.nextProductBtn, TextComands.nextProductBtn),

                },
                // second row
                new []
                {
                    InlineKeyboardButton.WithCallbackData(TextComands.backBtn,TextComands.backBtn),
                    InlineKeyboardButton.WithCallbackData(TextComands.addOrderBtn, TextComands.addOrderBtn),
                },
            });


        using (var fileStream = new FileStream(photoPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            return await bot.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: new InputFileStream(fileStream, Path.GetFileName(photoPath)),
                caption: "Here is your photo!",
                replyMarkup: inlineKeyboard
                
            );

           
        }

    }

    async Task SendPhotoAsync(long chatId, string photoPath)
    {
        string botToken = "1039876475:AAE6yf_CXTgnyyo99Pa1y7FPgIgm3JRRFeg";
        string url = $"https://api.telegram.org/bot{botToken}/sendPhoto";
        
        //string photoUrl = "https://disk.yandex.ru/i/9hfkr-gp4YWcKQ";
        string photoUrl = "http://localhost:8080/img/1.jpg";


        using var httpClient = new HttpClient();
        using (var formData = new MultipartFormDataContent())
        {
            formData.Add(new StringContent(chatId.ToString()), "chat_id");
            formData.Add(new StringContent("Наименование"), "caption");
            formData.Add(new StringContent(photoUrl), "photo");

            //formData.Add(new StreamContent(File.OpenRead(photoPath)), "photo", Path.GetFileName(photoPath));

            // InlineKeyboardMarkup Example
            var inlineKeyboard = new
            {
                inline_keyboard = new[]
                {
                    new[] { new { text = "Button 1", callback_data = "1" }, new { text = "Button 2", callback_data = "2" } },
                    new[] { new { text = "Button 3", callback_data = "3" } }
                }
            };

            var keyboardJson = JsonConvert.SerializeObject(inlineKeyboard);
            formData.Add(new StringContent(keyboardJson, Encoding.UTF8, "application/json"), "reply_markup");

            var response = await httpClient.PostAsync(url, formData);
            var responseString = await response.Content.ReadAsStringAsync();

            // Check if the request was successful
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send photo. Server responded with {response.StatusCode}: {responseString}");
            }

            Console.WriteLine("Photo sent successfully.");
        }



        return;




















        /*

        using (var httpClient = new HttpClient())
        {
            using (var form = new MultipartFormDataContent())
            {
                form.Add(new StringContent(chatId.ToString()), "chat_id");

                using (var fileStream = File.OpenRead(photoPath))
                {
                    var streamContent = new StreamContent(fileStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    form.Add(streamContent, "photo", Path.GetFileName(photoPath));

                    HttpResponseMessage response = await httpClient.PostAsync(url, form);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Photo sent successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send photo. Status code: {response.StatusCode}");
                    }
                }
            }
        }

        return;
        */
    }
}

