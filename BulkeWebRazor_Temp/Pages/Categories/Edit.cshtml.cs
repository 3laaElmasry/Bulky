
using BulkeWebRazor_Temp.Data;
using BulkeWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkeWebRazor_Temp.Pages.Categories
{

    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        
        public Category Category { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? categoryId)
        {
            if(categoryId is not null && categoryId is not 0)
            {
                Category = _db.categories.FirstOrDefault(c => c.Id == categoryId);
            }
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _db.Update(Category);
                _db.SaveChanges();
                TempData["Success"] = "Category Updated Successfully";
                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}
