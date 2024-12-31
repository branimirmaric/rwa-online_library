using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos
{
    public class UserReservationDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Reservation ID is required.")]
        public int ReservationId { get; set; }
    }
}
