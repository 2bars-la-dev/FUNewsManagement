using Microsoft.AspNetCore.Mvc.RazorPages;
using WebClient.Helpers;

public class LogoutModel : PageModel
{
    private readonly JwtService _jwtService;

    public LogoutModel(JwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public void OnGet()
    {
        Response.Cookies.Delete("access_token");
    }
}
