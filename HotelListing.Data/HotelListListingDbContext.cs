using HotelListing.API.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data
{
    public class HotelListListingDbContext : IdentityDbContext<ApiUser>
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

            modelBuilder.ApplyConfiguration(new RoleConfiguratiion());
            modelBuilder.ApplyConfiguration(new CountryConfiguratiion());
            modelBuilder.ApplyConfiguration(new HotelConfiguratiion());
        }
    }
}
