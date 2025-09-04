using System;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BackEnd.Data;

public class InnoviaHubDbContext : IdentityDbContext<AppUser>
{
    public InnoviaHubDbContext(DbContextOptions<InnoviaHubDbContext> options) : base(options)
    {

    }

    public DbSet<TimeSlots> TimeSlots { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Resources> Resources { get; set; }
    public DbSet<ResourceType> ResourceTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Skapa en hasher-instans
        var hasher = new PasswordHasher<AppUser>(); 

        // Skapa admin-användaren
        var adminUser = new AppUser
        {
            Id = "1", 
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

        // Skapa user-användaren
        var userUser = new AppUser 
        {
            Id = "2", 
            UserName = "user",
            NormalizedUserName = "USER",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
        };
        userUser.PasswordHash = hasher.HashPassword(userUser, "User123!");

        modelBuilder.Entity<AppUser>().HasData(adminUser, userUser);

        // Seeda roller
        var adminRole = new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" };
        var userRole = new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" };

        modelBuilder.Entity<IdentityRole>().HasData(adminRole, userRole);

        // Koppla admin-rollen till admin-användaren
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = adminUser.Id, RoleId = adminRole.Id },
            new IdentityUserRole<string> { UserId = userUser.Id, RoleId = userRole.Id }
        );

        modelBuilder.Entity<ResourceType>()
            .HasMany(rt => rt.Resources)
            .WithOne(r => r.ResourceType)
            .HasForeignKey(r => r.ResourceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Resources>()
            .HasMany(r => r.Bookings)
            .WithOne(b => b.Resource)
            .HasForeignKey(b => b.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeSlots>()
            .HasMany(ts => ts.Bookings)
            .WithOne(b => b.TimeSlot)
            .HasForeignKey(b => b.TimeSlotId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TimeSlots>().HasData(
            new TimeSlots { TimeSlotsId = 1, startTime = new TimeSpan(8, 0, 0), endTime = new TimeSpan(10, 0, 0), Duration = 2 },
            new TimeSlots { TimeSlotsId = 2, startTime = new TimeSpan(10, 0, 0), endTime = new TimeSpan(12, 0, 0), Duration = 2 },
            new TimeSlots { TimeSlotsId = 3, startTime = new TimeSpan(12, 0, 0), endTime = new TimeSpan(14, 0, 0), Duration = 2 },
            new TimeSlots { TimeSlotsId = 4, startTime = new TimeSpan(14, 0, 0), endTime = new TimeSpan(16, 0, 0), Duration = 2 },

            new TimeSlots { TimeSlotsId = 5, startTime = new TimeSpan(8, 0, 0), endTime = new TimeSpan(12, 0, 0), Duration = 4 },
            new TimeSlots { TimeSlotsId = 6, startTime = new TimeSpan(12, 0, 0), endTime = new TimeSpan(16, 0, 0), Duration = 4 },

            new TimeSlots { TimeSlotsId = 7, startTime = new TimeSpan(8, 0, 0), endTime = new TimeSpan(16, 0, 0), Duration = 8 }
        );

        modelBuilder.Entity<ResourceType>().HasData(
            new ResourceType { ResourceTypeId = 1, Name = "Drop-in skrivbord" },
            new ResourceType { ResourceTypeId = 2, Name = "Mötesrum" },
            new ResourceType { ResourceTypeId = 3, Name = "VR" },
            new ResourceType { ResourceTypeId = 4, Name = "AI" }
        );

        modelBuilder.Entity<Resources>().HasData(
            new Resources { ResourcesId = 1, ResourceTypeId = 1, Name = "Skrivbord 1" },
            new Resources { ResourcesId = 2, ResourceTypeId = 1, Name = "Skrivbord 2" },
            new Resources { ResourcesId = 3, ResourceTypeId = 1, Name = "Skrivbord 3" },
            new Resources { ResourcesId = 4, ResourceTypeId = 1, Name = "Skrivbord 4" },
            new Resources { ResourcesId = 5, ResourceTypeId = 1, Name = "Skrivbord 5" },
            new Resources { ResourcesId = 6, ResourceTypeId = 1, Name = "Skrivbord 6" },
            new Resources { ResourcesId = 7, ResourceTypeId = 1, Name = "Skrivbord 7" },
            new Resources { ResourcesId = 8, ResourceTypeId = 1, Name = "Skrivbord 8" },
            new Resources { ResourcesId = 9, ResourceTypeId = 1, Name = "Skrivbord 9" },
            new Resources { ResourcesId = 10, ResourceTypeId = 1, Name = "Skrivbord 10" },
            new Resources { ResourcesId = 11, ResourceTypeId = 1, Name = "Skrivbord 11" },
            new Resources { ResourcesId = 12, ResourceTypeId = 1, Name = "Skrivbord 12" },
            new Resources { ResourcesId = 13, ResourceTypeId = 1, Name = "Skrivbord 13" },
            new Resources { ResourcesId = 14, ResourceTypeId = 1, Name = "Skrivbord 14" },
            new Resources { ResourcesId = 15, ResourceTypeId = 1, Name = "Skrivbord 15" },

            new Resources { ResourcesId = 16, ResourceTypeId = 2, Name = "Mötesrum 1" },
            new Resources { ResourcesId = 17, ResourceTypeId = 2, Name = "Mötesrum 2" },
            new Resources { ResourcesId = 18, ResourceTypeId = 2, Name = "Mötesrum 3" },
            new Resources { ResourcesId = 19, ResourceTypeId = 2, Name = "Mötesrum 4" },

            new Resources { ResourcesId = 20, ResourceTypeId = 3, Name = "VR 1" },
            new Resources { ResourcesId = 21, ResourceTypeId = 3, Name = "VR 2" },
            new Resources { ResourcesId = 22, ResourceTypeId = 3, Name = "VR 3" },
            new Resources { ResourcesId = 23, ResourceTypeId = 3, Name = "VR 4" },

            new Resources { ResourcesId = 24, ResourceTypeId = 4, Name = "AI" }
        );
    }
}
