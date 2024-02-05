using DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Context
{
    public class DBContext: IdentityDbContext<User>
    {
        public DBContext() { }
        public DBContext(DbContextOptions options)
       : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }    
     
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);


        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Seller",
                    NormalizedName = "SELLER",
                    ConcurrencyStamp=Guid.NewGuid().ToString()
                },
                new IdentityRole
                {
                    Name = "Buyer",
                    NormalizedName = "BUYER",
                    ConcurrencyStamp=Guid.NewGuid().ToString()
        },
            };
            builder.Entity<IdentityRole>().HasData(roles);


        }
    }
}
