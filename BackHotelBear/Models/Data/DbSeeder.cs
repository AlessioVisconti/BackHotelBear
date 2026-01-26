using BackHotelBear.Models.Entity;
using Microsoft.AspNetCore.Identity;

namespace BackHotelBear.Models.Data
{
    public class DbSeeder
    {
        public static async Task SeedRoleAndUser(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string[] roles =
            {
                "Admin","Receptionist","RoomStaff","Customer"
            };
            foreach(var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            var adminEmail = "alextheadmin@adminemail.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new User
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    FirstName = "Alex",
                    LastName = "Bear",
                    Role = "Admin"
                };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
