using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebClient.Models;

namespace WebClient.Pages.Articles
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private readonly JwtService _jwtService;

        public IndexModel(IHttpClientFactory httpFactory, IConfiguration config, JwtService jwtService)
        {
            _httpFactory = httpFactory;
            _config = config;
            _jwtService = jwtService;
        }

        public List<NewsArticleModel> Articles { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public async Task OnGetAsync()
        {
            var client = _httpFactory.CreateClient();
            var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/NewsArticles");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to load articles");
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<List<NewsArticleModel>>();
            if (result == null)
                return;

            if (StartDate.HasValue)
                result = result.Where(a => a.CreatedDate >= StartDate.Value).ToList();
            if (EndDate.HasValue)
                result = result.Where(a => a.CreatedDate <= EndDate.Value).ToList();

            Articles = result;
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var token = _jwtService.GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Auth/Login");

            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"{_config["ApiBaseUrl"]}/NewsArticles/{id}");
            return RedirectToPage();
        }
    }
}
