using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class UserReservation
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ReservationId { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
