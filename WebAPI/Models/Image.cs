using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Image
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
