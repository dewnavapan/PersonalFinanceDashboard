using PersonalFinance.Application.ViewModels.Category;
using PersonalFinance.Core.Entities;

namespace PersonalFinance.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesByUserIdAsync(Guid userId);
        Task SaveCategoryAsync(CategoryFormViewModel model, Guid userId);
        Task DeleteCategoryAsync(Guid id, Guid userId);
    }
}