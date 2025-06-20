using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebClient.Models;

namespace WebClient.Pages.Articles
{
    public class UpdateModel : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private readonly JwtService _jwtService;

        public UpdateModel(IHttpClientFactory httpFactory, IConfiguration config, JwtService jwtService)
        {
            _httpFactory = httpFactory;
            _config = config;
            _jwtService = jwtService;
        }

        [BindProperty]
        public NewsArticleModel Article { get; set; } = new();

        public List<CategoryModel> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadCategoriesAsync();

            var token = _jwtService.GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Auth/Login");

            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/NewsArticles/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<NewsArticleModel>();
                if (data != null)
                    Article = data;
            }
            else
            {
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var token = _jwtService.GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Auth/Login");

            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync($"{_config["ApiBaseUrl"]}/NewsArticles/{id}", Article);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode}, content: {content}");
            }
        }

        private async Task LoadCategoriesAsync()
        {
            var token = Request.Cookies["access_token"];
            var client = _httpFactory.CreateClient();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/Categories");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<List<CategoryModel>>();
                if (data != null)
                    Categories = data;
            }
        }
    }
}
