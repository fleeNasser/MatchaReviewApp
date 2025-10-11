// Data/DbInitializer.cs
using MatchaReviewApp.Data;
using MatchaReviewApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MatchaReview.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if we already have data
            if (context.Stores.Any())
            {
                return; // DB has been seeded
            }

            // Create Roles
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create Admin User
            var adminUser = new User
            {
                UserName = "admin@matchareview.com",
                Email = "admin@matchareview.com",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                JoinDate = DateTime.Now.AddDays(-90)
            };

            var adminPassword = "User@123";
            var adminResult = await userManager.FindByEmailAsync(adminUser.Email);
            if (adminResult == null)
            {
                await userManager.CreateAsync(adminUser, adminPassword);
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Create Regular Users
            var user1 = new User
            {
                UserName = "john@example.com",
                Email = "john@example.com",
                EmailConfirmed = true,
                FirstName = "John",
                LastName = "Smith",
                JoinDate = DateTime.Now.AddDays(-75)
            };

            var user2 = new User
            {
                UserName = "sarah@example.com",
                Email = "sarah@example.com",
                EmailConfirmed = true,
                FirstName = "Sarah",
                LastName = "Johnson",
                JoinDate = DateTime.Now.AddDays(-60)
            };

            var user3 = new User
            {
                UserName = "mike@example.com",
                Email = "mike@example.com",
                EmailConfirmed = true,
                FirstName = "Mike",
                LastName = "Chen",
                JoinDate = DateTime.Now.AddDays(-45)
            };

            var userPassword = "User@123";

            foreach (var user in new[] { user1, user2, user3 })
            {
                var userResult = await userManager.FindByEmailAsync(user.Email);
                if (userResult == null)
                {
                    await userManager.CreateAsync(user, userPassword);
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            // Seed Stores
            var stores = new Store[]
            {
                new Store
                {
                    Name = "Tokyo Matcha Cafe",
                    Address = "123 George St, Sydney NSW 2000",
                    Rating = 4.5m,
                    Description = "Authentic Japanese matcha cafe with traditional tea ceremonies and modern matcha lattes. Uses premium Uji matcha from Kyoto.",
                    CreatedAt = DateTime.Now.AddDays(-60)
                },
                new Store
                {
                    Name = "Green Tea House",
                    Address = "45 Oxford St, Darlinghurst NSW 2010",
                    Rating = 4.2m,
                    Description = "Cozy tea house specializing in organic matcha and traditional Japanese sweets. Perfect spot for afternoon tea.",
                    CreatedAt = DateTime.Now.AddDays(-45)
                },
                new Store
                {
                    Name = "Matcha Moments",
                    Address = "78 King St, Newtown NSW 2042",
                    Rating = 4.8m,
                    Description = "Trendy matcha bar offering creative matcha drinks, desserts, and Instagram-worthy presentations. Known for their matcha soft serve.",
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
                new Store
                {
                    Name = "Zen Matcha Lounge",
                    Address = "12 Bondi Beach Rd, Bondi NSW 2026",
                    Rating = 4.0m,
                    Description = "Beachside matcha lounge with ocean views. Serves ceremonial grade matcha and healthy matcha bowls.",
                    CreatedAt = DateTime.Now.AddDays(-20)
                },
                new Store
                {
                    Name = "The Matcha Lab",
                    Address = "56 Crown St, Surry Hills NSW 2010",
                    Rating = 4.6m,
                    Description = "Experimental matcha cafe pushing boundaries with unique matcha creations. Try their matcha tiramisu!",
                    CreatedAt = DateTime.Now.AddDays(-15)
                },
                new Store
                {
                    Name = "Kyoto Corner",
                    Address = "89 Pitt St, Sydney NSW 2000",
                    Rating = 3.9m,
                    Description = "Small Japanese cafe offering traditional matcha tea service and light Japanese snacks.",
                    CreatedAt = DateTime.Now.AddDays(-10)
                }
            };

            context.Stores.AddRange(stores);
            await context.SaveChangesAsync();

            // Get user IDs for reviews
            var john = await userManager.FindByEmailAsync("john@example.com");
            var sarah = await userManager.FindByEmailAsync("sarah@example.com");
            var mike = await userManager.FindByEmailAsync("mike@example.com");

            // Seed Reviews
            var reviews = new Review[]
            {
                // Tokyo Matcha Cafe Reviews
                new Review
                {
                    StoreId = stores[0].Id,
                    UserId = john.Id,
                    Rating = 5,
                    Comment = "Absolutely amazing! The traditional matcha ceremony was a beautiful experience. The quality of their Uji matcha is unmatched in Sydney.",
                    CreatedAt = DateTime.Now.AddDays(-55)
                },
                new Review
                {
                    StoreId = stores[0].Id,
                    UserId = sarah.Id,
                    Rating = 4,
                    Comment = "Great matcha quality and lovely atmosphere. A bit pricey but worth it for special occasions.",
                    CreatedAt = DateTime.Now.AddDays(-50)
                },
                
                // Green Tea House Reviews
                new Review
                {
                    StoreId = stores[1].Id,
                    UserId = mike.Id,
                    Rating = 4,
                    Comment = "Cozy little spot with good matcha lattes. The Japanese sweets are delicious!",
                    CreatedAt = DateTime.Now.AddDays(-40)
                },
                new Review
                {
                    StoreId = stores[1].Id,
                    UserId = john.Id,
                    Rating = 5,
                    Comment = "Perfect place for a quiet afternoon. Love their organic matcha selection.",
                    CreatedAt = DateTime.Now.AddDays(-35)
                },

                // Matcha Moments Reviews
                new Review
                {
                    StoreId = stores[2].Id,
                    UserId = sarah.Id,
                    Rating = 5,
                    Comment = "The matcha soft serve is to die for! Beautiful presentation and amazing taste. My new favorite spot!",
                    CreatedAt = DateTime.Now.AddDays(-25)
                },
                new Review
                {
                    StoreId = stores[2].Id,
                    UserId = mike.Id,
                    Rating = 5,
                    Comment = "Innovative and delicious. Every drink is a work of art. Highly recommend the matcha affogato!",
                    CreatedAt = DateTime.Now.AddDays(-22)
                },
                new Review
                {
                    StoreId = stores[2].Id,
                    UserId = john.Id,
                    Rating = 4,
                    Comment = "Great vibe and tasty drinks. Can get crowded on weekends.",
                    CreatedAt = DateTime.Now.AddDays(-18)
                },

                // Zen Matcha Lounge Reviews
                new Review
                {
                    StoreId = stores[3].Id,
                    UserId = sarah.Id,
                    Rating = 4,
                    Comment = "Beautiful location with ocean views. The matcha bowls are healthy and delicious.",
                    CreatedAt = DateTime.Now.AddDays(-15)
                },
                new Review
                {
                    StoreId = stores[3].Id,
                    UserId = mike.Id,
                    Rating = 4,
                    Comment = "Perfect spot after a beach day. Good quality matcha at reasonable prices.",
                    CreatedAt = DateTime.Now.AddDays(-12)
                },

                // The Matcha Lab Reviews
                new Review
                {
                    StoreId = stores[4].Id,
                    UserId = john.Id,
                    Rating = 5,
                    Comment = "Mind-blowing matcha creations! The tiramisu is incredible. These guys really know their stuff.",
                    CreatedAt = DateTime.Now.AddDays(-10)
                },
                new Review
                {
                    StoreId = stores[4].Id,
                    UserId = sarah.Id,
                    Rating = 4,
                    Comment = "Very creative menu. Some experiments work better than others but overall excellent quality.",
                    CreatedAt = DateTime.Now.AddDays(-8)
                },

                // Kyoto Corner Reviews
                new Review
                {
                    StoreId = stores[5].Id,
                    UserId = mike.Id,
                    Rating = 4,
                    Comment = "Simple and authentic. Not fancy but good traditional matcha tea service.",
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new Review
                {
                    StoreId = stores[5].Id,
                    UserId = john.Id,
                    Rating = 3,
                    Comment = "Decent matcha but nothing special. Service was a bit slow.",
                    CreatedAt = DateTime.Now.AddDays(-3)
                }
            };

            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();

            // Recalculate store ratings based on reviews
            foreach (var store in stores)
            {
                var storeReviews = context.Reviews.Where(r => r.StoreId == store.Id).ToList();
                if (storeReviews.Any())
                {
                    store.Rating = (decimal)storeReviews.Average(r => r.Rating);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}