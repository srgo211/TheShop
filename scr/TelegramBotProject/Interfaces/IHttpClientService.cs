﻿using TelegramBotProject.Interfaces.Models;

namespace TelegramBotProject.Interfaces;

public interface IHttpClientService
{
    Task<HttpResponseMessage> GetAsync(string uri);
    Task<IProduct> GetProduct(int page, int itemsPerPage);


    Task<bool> CheckUser(long userId);
    Task<bool> AddUser(long userId);
    Task<SharedDomainModels.User> GetUserFromUserIdAsync(long userId);
    Task<bool> UpUser(long userId, string email);
}