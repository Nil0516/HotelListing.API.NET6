using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data
{
    public class HotelListListingDbContext : DbContext
    {
        // Cmd:
        //      add-migration InitalMigration
        //      update-database 
        //      Add-Migration SeededCountriedAndHotels
        //      update-database 

        public HotelListListingDbContext(DbContextOptions options) : base(options)
        {

        }
        
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        /// <summary>
        /// 當模型創建時新增測試資料。
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasData(
                    new Country { Id = 1, Name = "Jamaica", ShortName = "JM" }, 
                    new Country { Id = 2, Name = "Bahamas", ShortName = "BS" },
                    new Country { Id = 3, Name = "Cayman Island", ShortName = "CI" }
                );
            modelBuilder.Entity<Hotel>().HasData(
                    new Hotel { Id = 1, Name = "Sandalas Resort and Spa", Address = "Negril", CountryId = 1, Rating = 4.5 },
                    new Hotel { Id = 2, Name = "Comfort Suites", Address = "George Town", CountryId = 3, Rating = 4.3 },
                    new Hotel { Id = 3, Name = "Grand Pallddium", Address = "Nassua", CountryId = 2, Rating = 4 }
                ) ;
        }
    }
}
