using System.Net.Http;
using System;
using System.Net.Http.Headers;
using System.Text;
using File = System.IO.File;

namespace TelegramBotProject.Services;

public class HandleUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<HandleUpdateService> _logger;

    public HandleUpdateService(ITelegramBotClient botClient, ILogger<HandleUpdateService> logger)
    {
        _botClient = botClient;
        _logger = logger;
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
            UpdateType.CallbackQuery      => BotOnCallbackQueryReceived(update.CallbackQuery!),
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
        _logger.LogInformation("Receive message type: {messageType}", message.Type);
        if (message.Type != MessageType.Text)
            return;

        var action = message.Text!.Split(' ')[0] switch
        {
            TextComands.menu => MenuStore(message),
            TextComands.productСatalog => OtherMsg(message),
            TextComands.AdminPanel => OtherMsg(message),
            TextComands.muOrders => OtherMsg(message),
            TextComands.subscribe => OtherMsg(message),
            TextComands.unsubscribe => OtherMsg(message),
            "/inline" => SendInlineKeyboard(_botClient, message),
            "/keyboard" => SendReplyKeyboard(_botClient, message),
            "/remove" => RemoveKeyboard(_botClient, message),
            "/photo" => SendFile(_botClient, message),
            "/request" => RequestContactAndLocation(_botClient, message),
            _ => OtherMsg(message)
        };
        Message sentMessage = await action;
        _logger.LogInformation("The message was sent with id: {sentMessageId}", sentMessage.MessageId);

    
        async Task<Message> SendInlineKeyboard(ITelegramBotClient bot, Message message)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            // Simulate longer running task
            await Task.Delay(500);

            const string filePath = @"D:\Test\Images\1.jpg";
            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

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
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

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

    
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
    {
        await _botClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            text: $"Received {callbackQuery.Data}");

        await _botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message.Chat.Id,
            text: $"Received {callbackQuery.Data}");
    }

    #region Inline Mode

    private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery)
    {
        _logger.LogInformation("Received inline query from: {inlineQueryFromId}", inlineQuery.From.Id);

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

        await _botClient.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
                                                results: results,
                                                isPersonal: true,
                                                cacheTime: 0);
    }

    private Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)
    {
        _logger.LogInformation("Received inline result: {chosenInlineResultId}", chosenInlineResult.ResultId);
        return Task.CompletedTask;
    }

    #endregion

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {updateType}", update.Type);
        return Task.CompletedTask;
    }

    public Task HandleErrorAsync(Exception exception)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        return Task.CompletedTask;
    }



    async Task<Message> MenuStore(Message message)
    {
        string text = "Добро пожаловать в бот интернет магазина!";
        ReplyKeyboardMarkup replyKeyboardMarkup = new(
            new[]
            {
                new KeyboardButton[] { TextComands.productСatalog, TextComands.muOrders },
                new KeyboardButton[] { TextComands.subscribe, TextComands.unsubscribe },
                new KeyboardButton[] {TextComands.AdminPanel},
            })
        {
            ResizeKeyboard = true
        };




        return await _botClient.SendTextMessageAsync(chatId: message.Chat.Id,
             text: text,
             replyMarkup: replyKeyboardMarkup);
    }


    async Task<Message> OtherMsg(Message message)
    {
        string url = "https://disk.yandex.ru/i/9hfkr-gp4YWcKQ";
        //string url = "http://localhost:8080/img/1.jpg";
        string link = $"<a href=\"{url}\">link</a>";
        string text = "Нам пока не нужны эти данные! Спасибо!\n" + link;

        return await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: text, parseMode: ParseMode.Html,
            disableWebPagePreview:true);
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
            return await _botClient.SendPhotoAsync(
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

