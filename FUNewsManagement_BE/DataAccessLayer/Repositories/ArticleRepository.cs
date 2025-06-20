using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ArticleRepository
    {
        private readonly FunewsManagementContext _context;

        public ArticleRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public async Task<NewsArticle?> GetByIdAsync(string id)
        {
            return await _context.NewsArticles.Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.NewsArticleId == id);
        }

        public async Task<IEnumerable<NewsArticle>> GetAllAsync()
        {
            return await _context.NewsArticles.Include(c => c.Category).ToListAsync();
        }

        public async Task AddAsync(NewsArticle article)
        {
            await _context.NewsArticles.AddAsync(article);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message); // 👈 Dòng này sẽ cho bạn lý do thật
                throw;
            }
        }

        public async Task UpdateAsync(NewsArticle article)
        {
            _context.NewsArticles.Update(article);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(string articleId)
        {
            var article = await _context.NewsArticles.FindAsync(articleId);
            if (article == null)
            {
                return false;
            }

            _context.NewsArticles.Remove(article);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
