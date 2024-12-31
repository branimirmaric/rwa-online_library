using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos
{
    public class GenreDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You need to enter the genre.")]
        [StringLength(256, ErrorMessage = "Genre cannot exceed 256 characters.")]
        public string Name { get; set; } = null!;
    }
}
