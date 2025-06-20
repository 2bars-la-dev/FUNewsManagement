using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using WebClient.Models;

namespace WebClient.Pages.Articles
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public DetailsModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public NewsArticleModel Article { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/NewsArticles/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var article = await response.Content.ReadFromJsonAsync<NewsArticleModel>();
            if (article == null)
                return NotFound();

            Article = article;
            return Page();
        }
    }
}
