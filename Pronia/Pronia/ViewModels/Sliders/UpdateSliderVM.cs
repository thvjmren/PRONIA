using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
    public class UpdateSliderVM
    {
        [MaxLength(200, ErrorMessage = "title name must be less than 200 characters")]
        public string Title { get; set; }

        [MaxLength(200, ErrorMessage = "subtitle name must be less than 200 characters")]
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
