using System;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BackEnd.Data;

public class InnoviaHubDbContext : IdentityDbContext<IdentityUser>
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
        var hasher = new PasswordHasher<IdentityUser>(); 

        // Skapa admin-användaren
        var adminUser = new IdentityUser
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

        modelBuilder.Entity<IdentityUser>().HasData(adminUser);

        modelBuilder.Entity<IdentityUserClaim<string>>().HasData(new IdentityUserClaim<string>
        {
        Id = 1,
        UserId = "1",
        ClaimType = "role",
        ClaimValue = "admin"
        });

        // Skapa user-användaren
        var userUser = new IdentityUser 
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

        modelBuilder.Entity<IdentityUser>().HasData(userUser);

         modelBuilder.Entity<ResourceType>()
            .HasMany(rt => rt.Resources)
            .WithOne(r => r.ResourceType)
            .HasForeignKey(r => r.ResourceTypeId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of ResourceType if Resources exist.

        modelBuilder.Entity<Resources>()
            .HasMany(r => r.Bookings)
            .WithOne(b => b.Resource)
            .HasForeignKey(b => b.ResourceId)
            .OnDelete(DeleteBehavior.Cascade); // When a Resource is deleted, its Bookings are also deleted.

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeSlots>().HasData(
            new TimeSlots { Id = 1, startTime = new TimeSpan(8, 0, 0), endTime = new TimeSpan(10, 0, 0), Duration = 2 },
            new TimeSlots { Id = 2, startTime = new TimeSpan(10, 0, 0), endTime = new TimeSpan(12, 0, 0), Duration = 2 },
            new TimeSlots { Id = 3, startTime = new TimeSpan(12, 0, 0), endTime = new TimeSpan(14, 0, 0), Duration = 2 },
            new TimeSlots { Id = 4, startTime = new TimeSpan(14, 0, 0), endTime = new TimeSpan(16, 0, 0), Duration = 2 },

            new TimeSlots { Id = 5, startTime = new TimeSpan(8, 0, 0), endTime = new TimeSpan(12, 0, 0), Duration = 4 },
            new TimeSlots { Id = 6, startTime = new TimeSpan(12, 0, 0), endTime = new TimeSpan(16, 0, 0), Duration = 4 },

            new TimeSlots { Id = 7, startTime = new TimeSpan(8, 0, 0), endTime = new TimeSpan(16, 0, 0), Duration = 8 }
        );

        modelBuilder.Entity<ResourceType>().HasData(
            new ResourceType { Id = 1, Name = "Drop-in skrivbord" },
            new ResourceType { Id = 2, Name = "Mötesrum" },
            new ResourceType { Id = 3, Name = "VR" },
            new ResourceType { Id = 4, Name = "AI" }
        );

        modelBuilder.Entity<Resources>().HasData(
            new Resources { Id = 1, ResourceTypeId = 1, Name = "Skrivbord 1" },
            new Resources { Id = 2, ResourceTypeId = 1, Name = "Skrivbord 2" },
            new Resources { Id = 3, ResourceTypeId = 1, Name = "Skrivbord 3" },
            new Resources { Id = 4, ResourceTypeId = 1, Name = "Skrivbord 4" },
            new Resources { Id = 5, ResourceTypeId = 1, Name = "Skrivbord 5" },
            new Resources { Id = 6, ResourceTypeId = 1, Name = "Skrivbord 6" },
            new Resources { Id = 7, ResourceTypeId = 1, Name = "Skrivbord 7" },
            new Resources { Id = 8, ResourceTypeId = 1, Name = "Skrivbord 8" },
            new Resources { Id = 9, ResourceTypeId = 1, Name = "Skrivbord 9" },
            new Resources { Id = 10, ResourceTypeId = 1, Name = "Skrivbord 10" },
            new Resources { Id = 11, ResourceTypeId = 1, Name = "Skrivbord 11" },
            new Resources { Id = 12, ResourceTypeId = 1, Name = "Skrivbord 12" },
            new Resources { Id = 13, ResourceTypeId = 1, Name = "Skrivbord 13" },
            new Resources { Id = 14, ResourceTypeId = 1, Name = "Skrivbord 14" },
            new Resources { Id = 15, ResourceTypeId = 1, Name = "Skrivbord 15" },

            new Resources { Id = 16, ResourceTypeId = 2, Name = "Mötesrum 1" },
            new Resources { Id = 17, ResourceTypeId = 2, Name = "Mötesrum 2" },
            new Resources { Id = 18, ResourceTypeId = 2, Name = "Mötesrum 3" },
            new Resources { Id = 19, ResourceTypeId = 2, Name = "Mötesrum 4" },

            new Resources { Id = 20, ResourceTypeId = 3, Name = "VR 1" },
            new Resources { Id = 21, ResourceTypeId = 3, Name = "VR 2" },
            new Resources { Id = 22, ResourceTypeId = 3, Name = "VR 3" },
            new Resources { Id = 23, ResourceTypeId = 3, Name = "VR 4" },

            new Resources { Id = 24, ResourceTypeId = 4, Name = "AI" }
        );
    }

}
