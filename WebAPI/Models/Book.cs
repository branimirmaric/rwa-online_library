using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Book
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public int GenreId { get; set; }

    public string Description { get; set; } = null!;

    public string Isbn { get; set; } = null!;

    public string Availability { get; set; } = null!;

    public int? ImageId { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<BookLocation> BookLocations { get; set; } = new List<BookLocation>();

    public virtual Genre Genre { get; set; } = null!;

    public virtual Image? Image { get; set; }
}
