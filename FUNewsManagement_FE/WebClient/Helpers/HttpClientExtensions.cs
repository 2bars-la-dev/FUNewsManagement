using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace WebClient.Helpers
{
    public static class HttpClientExtensions
    {
        public static void AddJwtBearer(this HttpClient client, HttpContext context, string cookieName = "jwtToken")
        {
            if (context.Request.Cookies.TryGetValue(cookieName, out var token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
