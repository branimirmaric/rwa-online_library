using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos
{
    public class LocationDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You need to enter the state.")]
        [StringLength(256, ErrorMessage = "State cannot exceed 256 characters.")]
        public string State { get; set; } = null!;

        [Required(ErrorMessage = "You need to enter the city.")]
        [StringLength(256, ErrorMessage = "City cannot exceed 256 characters.")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "You need to enter the address.")]
        [StringLength(256, ErrorMessage = "Address cannot exceed 256 characters.")]
        public string Address { get; set; } = null!;
    }
}
