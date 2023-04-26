using System.Diagnostics;

namespace HotPotato.API;

public static class ActivityEnricher
{
    public static void EnrichHttpRequest(Activity activity, HttpRequest request)
    {
        var context = request.HttpContext;
        activity.AddTag("http.flavor", GetHttpFlavour(request.Protocol));
        activity.AddTag("http.scheme", request.Scheme);
        activity.AddTag("http.client_ip", context.Connection.RemoteIpAddress);
        activity.AddTag("http.request_content_length", request.ContentLength);
        activity.AddTag("http.request_content_type", request.ContentType);
    }
    
    public static void EnrichHttpResponse(Activity activity, HttpResponse response)
    {
        activity.AddTag("http.response_content_length", response.ContentLength);
        activity.AddTag("http.response_content_type", response.ContentType);
    }
    
    private static string GetHttpFlavour(string protocol)
    {
        if (HttpProtocol.IsHttp10(protocol))
        {
            return "1.0";
        }

        if (HttpProtocol.IsHttp11(protocol))
        {
            return "1.1";
        }

        if (HttpProtocol.IsHttp2(protocol))
        {
            return "2.0";
        }

        if (HttpProtocol.IsHttp3(protocol))
        {
            return "3.0";
        }

        throw new InvalidOperationException($"Protocol {protocol} not recognised.");
    }
}