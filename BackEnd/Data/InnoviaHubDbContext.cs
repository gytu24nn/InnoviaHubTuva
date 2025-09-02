using System;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data;

public class InnoviaHubDbContext : DbContext
{
    public InnoviaHubDbContext(DbContextOptions<InnoviaHubDbContext> options) : base(options)
    {

    }

    public DbSet<TimeSlots> TimeSlots { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TimeSlots>().HasData(
            new TimeSlots { Id = 1, startTime = new TimeSpan(8, 0, 0), endTime = new TimeSpan(10, 0, 0), Duration = 2 },
            new TimeSlots { Id = 2, startTime = new TimeSpan(10, 0, 0), endTime = new TimeSpan(12, 0, 0), Duration = 2 },
            new TimeSlots { Id = 3, startTime = new TimeSpan(12, 0, 0), endTime = new TimeSpan(14, 0, 0), Duration = 2 },
            new TimeSlots { Id = 4, startTime = new TimeSpan(14, 0, 0), endTime = new TimeSpan(16, 0, 0), Duration = 2 },

            new TimeSlots { Id = 5, startTime = new TimeSpan(8, 0, 0), endTime = new TimeSpan(12, 0, 0), Duration = 4 },
            new TimeSlots { Id = 6, startTime = new TimeSpan(12, 0, 0), endTime = new TimeSpan(16, 0, 0), Duration = 4 },

            new TimeSlots { Id = 7, startTime = new TimeSpan(8, 0, 0), endTime = new TimeSpan(16, 0, 0), Duration = 8 }
        );
    }

}
