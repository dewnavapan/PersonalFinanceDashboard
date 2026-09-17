using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.ViewModels.Category;
using PersonalFinance.Core.Entities;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesByUserIdAsync(Guid userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Type)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task SaveCategoryAsync(CategoryFormViewModel model, Guid userId)
        {
            var category = new Category
            {
                UserId = userId,
                Name = model.Name,
                Type = model.Type,
                Icon = model.Icon,
                Color = model.Color
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Guid id, Guid userId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null) throw new Exception("ไม่พบหมวดหมู่ที่ต้องการลบ");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}