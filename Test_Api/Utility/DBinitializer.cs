using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Test_Api.Datebase;
using Test_Api.Models;

namespace Test_Api.Utility
{
    public class DBinitializer : IDBinitializer
    {
        private readonly ApplicationDBcontext _context;
        private readonly ILogger<DBinitializer> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DBinitializer(ApplicationDBcontext context,ILogger<DBinitializer> logger,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetAppliedMigrations().Any())
                {
                    _context.Database.Migrate();
                }
                if ( !_roleManager.Roles.Any())
                {
                    _roleManager.CreateAsync(new(SD.Role_Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Role_Employee)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Role_User)).GetAwaiter().GetResult();
                    _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "Admin",
                        Email = "Admin123@gmail.com",
                        FirstName = "Admin",
                        LastName = "admin",
                        PhoneNumber = "1234567890",
                        Address = "Admin Address",
                        City = "Admin City",
                        EmailConfirmed = true,
                    }, "Admin@123").GetAwaiter().GetResult();
                }
               
                var user = _userManager.FindByEmailAsync("Admin123@gmail.com").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user!, SD.Role_Admin);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }
    }
}
