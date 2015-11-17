using AspNetRedisPersistedUserSession.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AspNetRedisPersistedUserSession.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        public AppDbContext():base("DefaultConnection")
        {
            
        }
    }
}