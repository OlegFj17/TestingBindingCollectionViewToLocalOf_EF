using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    public class DbCtx : DbContext
    {
        public DbSet<Shop> Shops { get; set; }
        public DbCtx()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AAA3;Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shop>().HasData(new Shop[]{
                new Shop{ShopId=1, Name="Shop1", Address="AAA"},
                new Shop{ShopId=2, Name="Shop2", Address="BBB"},
                new Shop{ShopId=3, Name="Shop3", Address="CCC"},
                new Shop{ShopId=4, Name="Shop4", Address="DDD"},
                new Shop{ShopId=5, Name="Shop5", Address="EEE"}
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
