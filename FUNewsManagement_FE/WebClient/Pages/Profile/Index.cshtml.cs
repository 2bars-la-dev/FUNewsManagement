using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using WebClient.Models;

namespace WebClient.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public AccountModel Account { get; set; } = new();

        [BindProperty]
        public string? NewPassword { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userId))
                return RedirectToPage("/Auth/Login");

            var client = _httpClientFactory.CreateClient();
            var apiUrl = $"{_config["ApiBaseUrl"]}/accounts/{userId}";
            var response = await client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = $"API error: {response.StatusCode}";
                return RedirectToPage("/Error");
            }

            Account = await response.Content.ReadFromJsonAsync<AccountModel>() ?? new();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userId))
                return RedirectToPage("/Auth/Login");

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                Account.AccountPassword = NewPassword;
            }

            var client = _httpClientFactory.CreateClient();
            var apiUrl = $"{_config["ApiBaseUrl"]}/accounts/{userId}";
            var response = await client.PutAsJsonAsync(apiUrl, Account);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Update failed.");
                return Page();
            }

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToPage();
        }
    }
}
