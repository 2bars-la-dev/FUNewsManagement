using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using WebClient.Models;
using System.Text.Json;

namespace WebClient.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public List<CategoryModel> Categories { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(short id)
        {
            var token = Request.Cookies["access_token"];
            var client = _httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"{_config["ApiBaseUrl"]}/Categories/{id}");

            if (response.IsSuccessStatusCode)
                return new JsonResult(new { success = true });

            var error = await response.Content.ReadAsStringAsync();
            return new JsonResult(new { success = false, error });
        }

        private async Task LoadCategoriesAsync()
        {
            var token = Request.Cookies["access_token"];
            var client = _httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/Categories");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<CategoryModel>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (result != null)
                        Categories = result;
                }
                else
                {
                    ErrorMessage = $"API Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Exception: {ex.Message}";
            }
        }
    }
}
