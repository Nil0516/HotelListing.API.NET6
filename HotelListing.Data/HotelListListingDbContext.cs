using HotelListing.API.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HotelListing.API.Data
{
    public class HotelListingDbContext : IdentityDbContext<ApiUser>
    {
        // Cmd:
        //      add-migration InitalMigration
        //      update-database 
        //      Add-Migration SeededCountriedAndHotels
        //      update-database 
        public HotelListingDbContext(DbContextOptions options) : base(options)
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

    public class HotelListingDbContextFactory : IDesignTimeDbContextFactory<HotelListingDbContext>
    {
        public HotelListingDbContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<HotelListingDbContext>();
            var conn = config.GetConnectionString("HotelListingDbConnectionString");
            optionsBuilder.UseSqlServer(conn);
            return new HotelListingDbContext(optionsBuilder.Options);
        }
    }
}
