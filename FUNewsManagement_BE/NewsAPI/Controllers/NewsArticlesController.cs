using BusinessLogicLayer;
using BusinessLogicLayer.DTOs;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Security.Claims;

namespace NewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsArticlesController : ODataController
    {
        private readonly ArticleService _articleService;

        public NewsArticlesController(ArticleService articleService)
        {
            _articleService = articleService;
        }

        // Publicly accessible (viewing articles)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var articles = await _articleService.GetAllAsync();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            var article = await _articleService.GetByIdAsync(id);

            if (article == null)
                return NotFound();

            var dto = new NewsArticleDTO
            {
                NewsArticleId = article.NewsArticleId,
                NewsTitle = article.NewsTitle,
                CreatedDate = article.CreatedDate,
                NewsContent = article.NewsContent,
                NewsSource = article.NewsSource,
                Category = article.Category == null ? null : new CategoryDTO
                {
                    CategoryId = article.Category.CategoryId,
                    CategoryName = article.Category.CategoryName
                }
            };

            return Ok(dto);
        }

        // Only Staff (role = 1) can create articles
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Post([FromBody] CreateNewsArticleDTO dto)
        {
            if (dto == null)
                return BadRequest("Article cannot be null.");

            try
            {
                short createdById = GetCurrentUserId();

                var article = new NewsArticle
                {
                    NewsArticleId = GenerateRandomId(),
                    NewsTitle = dto.NewsTitle,
                    Headline = dto.Headline,
                    NewsContent = dto.NewsContent,
                    NewsSource = dto.NewsSource,
                    CategoryId = dto.CategoryId,
                    NewsStatus = dto.NewsStatus ?? true, // Default to true if null
                    CreatedDate = DateTime.UtcNow,
                    CreatedById = createdById
                };

                await _articleService.AddAsync(article, createdById);

                return CreatedAtAction(nameof(GetById), new { id = article.NewsArticleId }, article);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Put(string id, [FromBody] CreateNewsArticleDTO dto)
        {
            try
            {
                short updaterId = GetCurrentUserId();

                var article = new NewsArticle
                {
                    NewsArticleId = id, // Gán từ route
                    NewsTitle = dto.NewsTitle,
                    Headline = dto.Headline,
                    NewsContent = dto.NewsContent,
                    NewsSource = dto.NewsSource,
                    CategoryId = dto.CategoryId,
                    NewsStatus = dto.NewsStatus,
                    ModifiedDate = DateTime.UtcNow
                };

                var result = await _articleService.UpdateAsync(id, article, updaterId);
                return result ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                short updaterId = GetCurrentUserId();
                var result = await _articleService.DeleteAsync(id, updaterId);
                return result ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private short GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !short.TryParse(userIdClaim, out var id))
            {
                throw new UnauthorizedAccessException("Cannot determine current user.");
            }

            return id;
        }

        private string GenerateRandomId(int length = 19)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
