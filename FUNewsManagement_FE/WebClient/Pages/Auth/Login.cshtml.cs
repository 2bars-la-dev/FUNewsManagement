using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class LoginModel : PageModel
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpFactory;

    public LoginModel(IConfiguration config, IHttpClientFactory httpFactory)
    {
        _config = config;
        _httpFactory = httpFactory;
    }

    [BindProperty] public string Email { get; set; }
    [BindProperty] public string Password { get; set; }
    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = _httpFactory.CreateClient();
        var response = await client.PostAsJsonAsync($"{_config["ApiBaseUrl"]}/auth/login", new
        {
            Email = Email.Trim(),
            Password = Password.Trim()
        });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var token = result["token"];

            HttpContext.Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            });

            return RedirectToPage("/Articles/Index");
        }

        var error = await response.Content.ReadAsStringAsync();
        ErrorMessage = $"Login failed: {error}";
        return Page();
    }
}
