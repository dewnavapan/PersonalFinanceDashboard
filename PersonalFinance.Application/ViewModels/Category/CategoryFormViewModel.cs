using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Application.ViewModels.Category
{
    public class CategoryFormViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "กรุณาระบุชื่อหมวดหมู่")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณาระบุประเภท")]
        public string Type { get; set; } = "Expense"; // Income หรือ Expense

        [MaxLength(50)]
        public string? Icon { get; set; }

        [MaxLength(20)]
        public string? Color { get; set; } = "#36A2EB"; // ค่าสี Default สำหรับแสดงผลกราฟ
    }
}