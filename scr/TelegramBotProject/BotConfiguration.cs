﻿namespace TelegramBotProject;

public class BotConfiguration
{
    public string? BotToken { get; init; }

    // Open API is unable to process urls with ":" symbol
    public string? EscapedBotToken => BotToken?.Replace(':', '_');

    public string? WebhookAddress { get; init; }
    public string? HostFilesAddress { get; init; }
    public string? HostAddressCatalogProduct { get; init; }
    public string? NotificationService   { get; init; }
}