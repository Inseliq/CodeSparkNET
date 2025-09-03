using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeSparkNET.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole{
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole{
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole{
                Name = "Prime",
                NormalizedName = "PRIME"
            }
        };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}