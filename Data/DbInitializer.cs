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
            // Ensure database is migrated to latest so schema (ImagePath) exists
            await context.Database.MigrateAsync();

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

            // Create Regular Users with concrete join dates
            var user1 = new User
            {
                UserName = "john@example.com",
                Email = "john@example.com",
                EmailConfirmed = true,
                FirstName = "John",
                LastName = "Smith",
                JoinDate = new DateTime(2024, 01, 15, 09, 00, 00, DateTimeKind.Utc)
            };

            var user2 = new User
            {
                UserName = "sarah@example.com",
                Email = "sarah@example.com",
                EmailConfirmed = true,
                FirstName = "Sarah",
                LastName = "Johnson",
                JoinDate = new DateTime(2024, 02, 01, 10, 00, 00, DateTimeKind.Utc)
            };

            var user3 = new User
            {
                UserName = "mike@example.com",
                Email = "mike@example.com",
                EmailConfirmed = true,
                FirstName = "Mike",
                LastName = "Chen",
                JoinDate = new DateTime(2024, 03, 01, 11, 00, 00, DateTimeKind.Utc)
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

            // Seed Stores with concrete CreatedAt dates (and optional ImagePath values if desired)
            var stores = new Store[]
            {
                new Store
                {
                    Name = "Tokyo Matcha Cafe",
                    Address = "123 George St, Sydney NSW 2000",
                    Rating = 4.5m,
                    Description = "Authentic Japanese matcha cafe with traditional tea ceremonies and modern matcha lattes. Uses premium Uji matcha from Kyoto.",
                    CreatedAt = new DateTime(2024, 04, 01, 10, 00, 00, DateTimeKind.Utc),
                    ImagePath = "/uploads/stores/tokyo-matcha-cafe.jpg"
                },
                new Store
                {
                    Name = "Green Tea House",
                    Address = "45 Oxford St, Darlinghurst NSW 2010",
                    Rating = 4.2m,
                    Description = "Cozy tea house specializing in organic matcha and traditional Japanese sweets. Perfect spot for afternoon tea.",
                    CreatedAt = new DateTime(2024, 04, 16, 11, 30, 00, DateTimeKind.Utc),
                    ImagePath = "/uploads/stores/green-tea-house.jpg"
                },
                new Store
                {
                    Name = "Matcha Moments",
                    Address = "78 King St, Newtown NSW 2042",
                    Rating = 4.8m,
                    Description = "Trendy matcha bar offering creative matcha drinks, desserts, and Instagram-worthy presentations. Known for their matcha soft serve.",
                    CreatedAt = new DateTime(2024, 04, 30, 14, 00, 00, DateTimeKind.Utc),
                    ImagePath = "/uploads/stores/matcha-moments.jpg"
                },
                new Store
                {
                    Name = "Zen Matcha Lounge",
                    Address = "12 Bondi Beach Rd, Bondi NSW 2026",
                    Rating = 4.0m,
                    Description = "Beachside matcha lounge with ocean views. Serves ceremonial grade matcha and healthy matcha bowls.",
                    CreatedAt = new DateTime(2024, 05, 05, 09, 15, 00, DateTimeKind.Utc),
                    ImagePath = "/uploads/stores/zen-matcha-lounge.jpg"
                },
                new Store
                {
                    Name = "The Matcha Lab",
                    Address = "56 Crown St, Surry Hills NSW 2010",
                    Rating = 4.6m,
                    Description = "Experimental matcha cafe pushing boundaries with unique matcha creations. Try their matcha tiramisu!",
                    CreatedAt = new DateTime(2024, 05, 10, 16, 45, 00, DateTimeKind.Utc),
                    ImagePath = "/uploads/stores/the-matcha-lab.jpg"
                },
                new Store
                {
                    Name = "Kyoto Corner",
                    Address = "89 Pitt St, Sydney NSW 2000",
                    Rating = 3.9m,
                    Description = "Small Japanese cafe offering traditional matcha tea service and light Japanese snacks.",
                    CreatedAt = new DateTime(2024, 05, 15, 12, 00, 00, DateTimeKind.Utc),
                    ImagePath = "/uploads/stores/kyoto-corner.jpg"
                }
            };

            context.Stores.AddRange(stores);
            await context.SaveChangesAsync();

            // Get user IDs for reviews
            var john = await userManager.FindByEmailAsync("john@example.com");
            var sarah = await userManager.FindByEmailAsync("sarah@example.com");
            var mike = await userManager.FindByEmailAsync("mike@example.com");

            // Seed Reviews with concrete CreatedAt dates
            var reviews = new Review[]
            {
                // Tokyo Matcha Cafe Reviews
                new Review
                {
                    StoreId = stores[0].Id,
                    UserId = john.Id,
                    Rating = 5,
                    Comment = "Absolutely amazing! The traditional matcha ceremony was a beautiful experience. The quality of their Uji matcha is unmatched in Sydney.",
                    CreatedAt = new DateTime(2024, 04, 05, 12, 00, 00, DateTimeKind.Utc)
                },
                new Review
                {
                    StoreId = stores[0].Id,
                    UserId = sarah.Id,
                    Rating = 4,
                    Comment = "Great matcha quality and lovely atmosphere. A bit pricey but worth it for special occasions.",
                    CreatedAt = new DateTime(2024, 04, 10, 15, 00, 00, DateTimeKind.Utc)
                },

                // Green Tea House Reviews
                new Review
                {
                    StoreId = stores[1].Id,
                    UserId = mike.Id,
                    Rating = 4,
                    Comment = "Cozy little spot with good matcha lattes. The Japanese sweets are delicious!",
                    CreatedAt = new DateTime(2024, 04, 20, 13, 30, 00, DateTimeKind.Utc)
                },
                new Review
                {
                    StoreId = stores[1].Id,
                    UserId = john.Id,
                    Rating = 5,
                    Comment = "Perfect place for a quiet afternoon. Love their organic matcha selection.",
                    CreatedAt = new DateTime(2024, 04, 25, 10, 45, 00, DateTimeKind.Utc)
                },

                // Matcha Moments Reviews
                new Review
                {
                    StoreId = stores[2].Id,
                    UserId = sarah.Id,
                    Rating = 5,
                    Comment = "The matcha soft serve is to die for! Beautiful presentation and amazing taste. My new favorite spot!",
                    CreatedAt = new DateTime(2024, 05, 01, 11, 15, 00, DateTimeKind.Utc)
                },
                new Review
                {
                    StoreId = stores[2].Id,
                    UserId = mike.Id,
                    Rating = 5,
                    Comment = "Innovative and delicious. Every drink is a work of art. Highly recommend the matcha affogato!",
                    CreatedAt = new DateTime(2024, 05, 03, 14, 00, 00, DateTimeKind.Utc)
                },
                new Review
                {
                    StoreId = stores[2].Id,
                    UserId = john.Id,
                    Rating = 4,
                    Comment = "Great vibe and tasty drinks. Can get crowded on weekends.",
                    CreatedAt = new DateTime(2024, 05, 06, 16, 30, 00, DateTimeKind.Utc)
                },

                // Zen Matcha Lounge Reviews
                new Review
                {
                    StoreId = stores[3].Id,
                    UserId = sarah.Id,
                    Rating = 4,
                    Comment = "Beautiful location with ocean views. The matcha bowls are healthy and delicious.",
                    CreatedAt = new DateTime(2024, 05, 08, 09, 45, 00, DateTimeKind.Utc)
                },
                new Review
                {
                    StoreId = stores[3].Id,
                    UserId = mike.Id,
                    Rating = 4,
                    Comment = "Perfect spot after a beach day. Good quality matcha at reasonable prices.",
                    CreatedAt = new DateTime(2024, 05, 10, 12, 00, 00, DateTimeKind.Utc)
                },

                // The Matcha Lab Reviews
                new Review
                {
                    StoreId = stores[4].Id,
                    UserId = john.Id,
                    Rating = 5,
                    Comment = "Mind-blowing matcha creations! The tiramisu is incredible. These guys really know their stuff.",
                    CreatedAt = new DateTime(2024, 05, 12, 17, 00, 00, DateTimeKind.Utc)
                },
                new Review
                {
                    StoreId = stores[4].Id,
                    UserId = sarah.Id,
                    Rating = 4,
                    Comment = "Very creative menu. Some experiments work better than others but overall excellent quality.",
                    CreatedAt = new DateTime(2024, 05, 13, 13, 30, 00, DateTimeKind.Utc)
                },

                // Kyoto Corner Reviews
                new Review
                {
                    StoreId = stores[5].Id,
                    UserId = mike.Id,
                    Rating = 4,
                    Comment = "Simple and authentic. Not fancy but good traditional matcha tea service.",
                    CreatedAt = new DateTime(2024, 05, 14, 11, 00, 00, DateTimeKind.Utc)
                },
                new Review
                {
                    StoreId = stores[5].Id,
                    UserId = john.Id,
                    Rating = 3,
                    Comment = "Decent matcha but nothing special. Service was a bit slow.",
                    CreatedAt = new DateTime(2024, 05, 16, 10, 00, 00, DateTimeKind.Utc)
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