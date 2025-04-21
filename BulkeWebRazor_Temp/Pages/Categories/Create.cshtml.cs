
using BulkeWebRazor_Temp.Data;
using BulkeWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkeWebRazor_Temp.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public Category Category { get; set; }
        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {

        }
    }
}
