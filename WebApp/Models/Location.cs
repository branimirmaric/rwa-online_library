using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Location
{
    public int Id { get; set; }

    public string State { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<BookLocation> BookLocations { get; set; } = new List<BookLocation>();
}
