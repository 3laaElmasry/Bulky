using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        [Range(1,1000,ErrorMessage = "Please Enter a value bewteen 1-1000")]
        public int Count { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product? Product { get; set; }

        public double Price { get; set; }

        [ForeignKey("ApplicationUser")]
        [Required]
        public string? ApplicationUserId { get; set; }

        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
