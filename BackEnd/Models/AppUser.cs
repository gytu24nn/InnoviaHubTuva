using System;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.Models;

public class AppUser : IdentityUser
{
    public ICollection<Booking> Bookings { get; set; }
}
