using System.Net.Http;

namespace Finance.Utils;

public class HttpClientFactoryWrapper : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        return HttpClientFactory.CreateClient();
    }
}

public static class HttpClientFactory
{
    static SocketsHttpHandler SocketsHttpHandler;
    static HttpClientFactory()
    {
        SocketsHttpHandler = new SocketsHttpHandler();
    }

    public static HttpClient CreateClient()
    {
        return new HttpClient(SocketsHttpHandler, false);
    }
}