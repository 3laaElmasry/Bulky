
using BulkeWebRazor_Temp.Data;
using BulkeWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkeWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {

        private readonly ApplicationDbContext _db;

        public Category Category { get; set; }
        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? categoryId)
        {
            if (categoryId is not null && categoryId is not 0)
            {
                Category = _db.categories.FirstOrDefault(c => c.Id == categoryId);
            }
        }

        public IActionResult OnPost(int? categoryId)
        {
            if(categoryId is null or < 1 or > 100)
            {
                return NotFound();
            }
            Category? categoryToDelete = _db.categories
                .FirstOrDefault(c => c.Id == categoryId);
            if (categoryToDelete is null)
            {
                return NotFound();
            }
            _db.Remove(categoryToDelete);
            _db.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}
