using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class ImageVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You need to enter the URL of image.")]
        public string Content { get; set; }
    }
}
