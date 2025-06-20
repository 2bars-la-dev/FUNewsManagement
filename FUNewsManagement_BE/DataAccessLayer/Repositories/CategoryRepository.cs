using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class CategoryRepository
    {
        private readonly FunewsManagementContext _context;

        public CategoryRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdAsync(short categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(short categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return false;
            }
            var children = await _context.Categories
    .Where(c => c.ParentCategoryId == categoryId)
    .ToListAsync();

            foreach (var child in children)
            {
                child.ParentCategoryId = null;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        //search for category by name or isactive
        public async Task<IEnumerable<Category>> SearchCategoriesAsync(string? name = null, bool? isActive = null)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.CategoryName.Contains(name));
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            return await query.ToListAsync();
        }
    }
}
