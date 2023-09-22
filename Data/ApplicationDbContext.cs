using MagicVillaApi.Models.Class;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Villa> Villas { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Villa>().HasData(
        //        new Villa()
        //        {
        //            Id = 1,
        //            Name = "Villa Test",
        //            Details = "Villa insertada para test",
        //            Tariff = 20,
        //            NumberOfOccupants = 200,
        //            SquareMeter = 2200,
        //            ImageUrl = "",
        //            Amenity = "",
        //            CreationDate = DateTime.Now,    
        //            UpdateDate = DateTime.Now
        //        }
        //    ) ;
        //}

    }
}
