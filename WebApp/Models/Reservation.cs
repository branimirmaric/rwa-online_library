using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Reservation
{
    public int Id { get; set; }

    public DateTime BorrowDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public int BookLocationId { get; set; }

    public virtual BookLocation BookLocation { get; set; } = null!;

    public virtual ICollection<UserReservation> UserReservations { get; set; } = new List<UserReservation>();
}
