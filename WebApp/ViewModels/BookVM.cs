using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class BookVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You need to enter the title.")]
        [StringLength(256, ErrorMessage = "Title cannot exceed 256 characters.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "You need to enter the author's name.")]
        [StringLength(256, ErrorMessage = "Author's name cannot exceed 256 characters.")]
        public string Author { get; set; }

        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        [Display(Name = "Genre")]
        [ValidateNever]
        public string GenreName { get; set; }

        [Required(ErrorMessage = "You need to enter the description.")]
        [StringLength(1024, ErrorMessage = "Description cannot exceed 1024 characters.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "You need to enter the ISBN.")]
        [StringLength(17, ErrorMessage = "ISBN cannot exceed 17 characters. Enter numbers in these pathern: xxx-xxx-xxx-xxx-x.")]
        [Display(Name = "ISBN")]
        public string Isbn { get; set; }
        [Required(ErrorMessage = "You need to enter the availability.")]
        [StringLength(50, ErrorMessage = "Availability status cannot exceed 50 characters.")]
        public string Availability { get; set; }
        [Display(Name = "Image")]
        [Required(ErrorMessage = "You need to choose the image.")]
        [Range(1, int.MaxValue, ErrorMessage = "Image must have a number.")]
        public int? ImageId { get; set; }
        [ValidateNever]
        public List<int> LocationIds { get; set; }
    }
}
