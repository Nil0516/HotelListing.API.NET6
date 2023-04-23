using AutoMapper;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Core.Repository
{
    public class HotelsRepository : GenericRepository<Hotel>, IHotelsRepository
    {
        private readonly HotelListListingDbContext _context;

        public HotelsRepository(HotelListListingDbContext context, IMapper mapper) : base(context, mapper)
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
