using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using WebClient.Models;

namespace WebClient.Pages.Accounts
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

        public List<AccountModel> Accounts { get; set; } = new();

        public async Task OnGetAsync()
        {
            var token = Request.Cookies["access_token"]; 

            var client = _httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/Accounts");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<AccountModel>>();
                if (result != null)
                {
                    Accounts = result;
                }
            }
        }
    }
}
