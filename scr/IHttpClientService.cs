using System;

public interface IHttpClientService
{
    Task<HttpResponseMessage> GetAsync(string uri);
}
