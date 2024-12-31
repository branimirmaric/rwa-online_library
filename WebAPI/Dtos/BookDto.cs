using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You need to enter the title.")]
        [StringLength(256, ErrorMessage = "Title cannot exceed 256 characters.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "You need to enter the author's name.")]
        [StringLength(256, ErrorMessage = "Author's name cannot exceed 256 characters.")]
        public string Author { get; set; } = null!;

        [Required(ErrorMessage = "You need to set a number of a genre. Look at genre table.")]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "You need to enter the description.")]
        [StringLength(1024, ErrorMessage = "Description cannot exceed 1024 characters.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "You need to enter the ISBN.")]
        [StringLength(17, ErrorMessage = "ISBN cannot exceed 17 characters. Enter numbers in these pathern: xxx-xxx-xxx-xxx-x.")]
        public string Isbn { get; set; } = null!;

        [Required(ErrorMessage = "You need to enter the availability.")]
        [StringLength(50, ErrorMessage = "Availability status cannot exceed 50 characters.")]
        public string Availability { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Image must have a number.")]
        public int? ImageId { get; set; }
    }
}
