using HotelListing.API.Contracts;
using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository
{
    public class HotelsRepository : GenericRepository<Hotel>, IHotelsRepository
    {
        private readonly HotelListListingDbContext _context;

        public HotelsRepository(HotelListListingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Hotel> GetHotelDetail(int id)
        {
            return await _context.Hotels.Include(q => q.Country)
                                        .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
