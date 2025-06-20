using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace BusinessLogicLayer
{
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepo;

        public CategoryService(CategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // Get all categories asynchronously
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _categoryRepo.GetAllAsync();
        }

        // Get category by ID asynchronously
        public async Task<Category?> GetByIdAsync(short categoryId)
        {
            return await _categoryRepo.GetByIdAsync(categoryId);
        }

        // Add a new category asynchronously
        public async Task AddAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            await _categoryRepo.AddAsync(category);
        }

        // Update an existing category asynchronously
        public async Task<bool> UpdateAsync(short id, Category updated)
        {
            var existing = await _categoryRepo.GetByIdAsync(id);
            if (existing == null) return false;

            // Update fields
            existing.CategoryName = updated.CategoryName;
            existing.IsActive = updated.IsActive;
            existing.NewsArticles = updated.NewsArticles;

            await _categoryRepo.UpdateAsync(existing);
            return true;
        }

        // Delete a category asynchronously
        public async Task<bool> DeleteAsync(short id)
        {
            return await _categoryRepo.DeleteAsync(id);
        }

        // Search for categories by name or isActive status
        public async Task<IEnumerable<Category>> SearchCategoriesAsync(string? name = null, bool? isActive = null)
        {
            return await _categoryRepo.SearchCategoriesAsync(name, isActive);
        }
    }
}
