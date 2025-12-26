using System;
using System.Collections.Generic;

namespace ShoeStoreLibrary.Models;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string? FirstName { get; set; }

    public string? SecondName { get; set; }

    public string? MiddleName { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;
}
