using MagicVillaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        public DbSet<Villa> Villas { get; set; }
        public DbSet<NumberVilla> NumberVillas { get; set; }
        public DbSet<User> Users { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>()
        //        .HasIndex(u => u.UserName)
        //        .IsUnique();
        //}
    }
}
