using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="The Name Can't be blank or empty")]
        [StringLength(30,ErrorMessage = "The Category Name should be between 1 and 30")]
        public string? Name { get; set; }

        [Range(minimum:1,maximum:100,
            ErrorMessage = "The Display Order should be 1-100")]
        public int DisplayOrder { get; set; }
    }
}
