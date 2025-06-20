public class JwtService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _config;

    public JwtService(IHttpContextAccessor httpContextAccessor, IConfiguration config)
    {
        _httpContextAccessor = httpContextAccessor;
        _config = config;
    }

    public string? GetToken()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies["access_token"];
    }

    public void SaveToken(string token)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(_config["Jwt:CookieName"], token,
            new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
    }

    public void ClearToken()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(_config["Jwt:CookieName"]);
    }
}
