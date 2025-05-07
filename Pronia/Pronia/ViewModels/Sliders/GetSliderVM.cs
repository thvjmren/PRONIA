using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
    public class GetSliderVM
    {
        public int Id { get; set; }
        [MaxLength(200, ErrorMessage = "title name must be less than 200 characters")]
        public string Title { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAT { get; set; }
    }
}
