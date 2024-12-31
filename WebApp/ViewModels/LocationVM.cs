using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class LocationVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You need to enter the state.")]
        public string State { get; set; }
        [Required(ErrorMessage = "You need to enter the city.")]
        public string City { get; set; }
        [Required(ErrorMessage = "You need to enter the address.")]
        public string Address { get; set; }
    }
}
