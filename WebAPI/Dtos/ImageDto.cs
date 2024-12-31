using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos
{
    public class ImageDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You need to enter the URL for the book's cover image.")]
        public string Content { get; set; } = null!;
    }
}
