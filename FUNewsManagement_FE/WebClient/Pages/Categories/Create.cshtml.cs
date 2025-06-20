using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WebClient.Models;

namespace WebClient.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [BindProperty]
        public CategoryModel Category { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var token = Request.Cookies["access_token"];
            var client = _httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var url = $"{_config["ApiBaseUrl"]}/Categories";
                var json = JsonSerializer.Serialize(Category);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"API Error: {response.StatusCode}. {errorBody}";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Exception: {ex.Message}";
                return Page();
            }
        }
    }
}
