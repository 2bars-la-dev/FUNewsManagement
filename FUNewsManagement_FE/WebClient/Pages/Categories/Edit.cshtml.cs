using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using WebClient.Models;

namespace WebClient.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public EditModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [BindProperty]
        public CategoryModel Category { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(short id)
        {
            var client = _httpClientFactory.CreateClient();
            var token = Request.Cookies["access_token"];

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/Categories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CategoryModel>();
                    if (result != null)
                        Category = result;
                    return Page();
                }
                else
                {
                    ErrorMessage = $"API Error: {response.StatusCode}";
                    return RedirectToPage("Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Exception: {ex.Message}";
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var client = _httpClientFactory.CreateClient();
            var token = Request.Cookies["access_token"];

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var json = JsonSerializer.Serialize(Category);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{_config["ApiBaseUrl"]}/Categories/{Category.CategoryId}", content);
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("Index");

                var errorBody = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"API Error: {response.StatusCode}. {errorBody}";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Exception: {ex.Message}";
                return Page();
            }
        }
    }
}
