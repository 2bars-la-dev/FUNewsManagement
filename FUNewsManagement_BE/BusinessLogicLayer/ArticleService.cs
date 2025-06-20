using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

public class ArticleService
{
    private readonly ArticleRepository _articleRepo;
    private readonly CategoryRepository _categoryRepo;

    public ArticleService(ArticleRepository articleRepo, CategoryRepository categoryRepo)
    {
        _articleRepo = articleRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task<IEnumerable<NewsArticle>> GetAllAsync()
    {
        return await _articleRepo.GetAllAsync();
    }

    public async Task<NewsArticle?> GetByIdAsync(string articleId)
    {
        return await _articleRepo.GetByIdAsync(articleId);
    }

    public async Task AddAsync(NewsArticle article, short authorId)
    {
        if (article == null) throw new ArgumentNullException(nameof(article));

        if (article.CategoryId.HasValue)
        {
            var category = await _categoryRepo.GetByIdAsync(article.CategoryId.Value);
            if (category == null || category.IsActive != true)
                    throw new ArgumentException("Invalid or inactive category.");
        }

        article.CreatedById = authorId;
        article.CreatedDate = DateTime.UtcNow;
        article.NewsStatus = true;

        await _articleRepo.AddAsync(article);
    }

    public async Task<bool> UpdateAsync(string id, NewsArticle updated, short updaterId)
    {
        var existing = await _articleRepo.GetByIdAsync(id);
        if (existing == null) return false;

        existing.NewsTitle = updated.NewsTitle;
        existing.NewsSource = updated.NewsSource;
        existing.Headline = updated.Headline;
        existing.NewsContent = updated.NewsContent;
        existing.CategoryId = updated.CategoryId;
        existing.NewsStatus = updated.NewsStatus;
        existing.ModifiedDate = DateTime.UtcNow;
        existing.UpdatedById = updaterId;

        await _articleRepo.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(string id, short updaterId)
    {
        var article = await _articleRepo.GetByIdAsync(id);
        if (article == null) return false;

        article.NewsStatus = false;
        article.ModifiedDate = DateTime.UtcNow;
        article.UpdatedById = updaterId;

        await _articleRepo.UpdateAsync(article);
        return true;
    }
}
