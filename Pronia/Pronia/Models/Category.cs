﻿using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Category : BaseEntity
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        //[MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        //relational
        public List<Product>? Products { get; set; }
    }
}
