using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class GenreVM
    {
        public int Id { get; set; }
        [Display(Name = "Genre name")]
        [Required(ErrorMessage = "You need to enter the genre name.")]
        public string Name { get; set; }
    }
}
