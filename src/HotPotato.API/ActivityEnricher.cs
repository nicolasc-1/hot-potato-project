using System.Diagnostics;

namespace HotPotato.API;

public static class ActivityEnricher
{
    public static void EnrichHttpRequest(Activity activity, HttpRequest request)
    {
        var context = request.HttpContext;
        activity.AddTag("http.client_ip", context.Connection.RemoteIpAddress);
        activity.AddTag("http.request_content_length", request.ContentLength);
        activity.AddTag("http.request_content_type", request.ContentType);
    }
    
    public static void EnrichHttpResponse(Activity activity, HttpResponse response)
    {
        activity.AddTag("http.response_content_length", response.ContentLength);
        activity.AddTag("http.response_content_type", response.ContentType);
    }
}