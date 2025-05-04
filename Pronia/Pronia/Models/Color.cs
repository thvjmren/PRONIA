using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Color : BaseEntity
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
