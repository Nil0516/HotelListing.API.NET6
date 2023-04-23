using HotelListing.API.Data;

namespace HotelListing.API.Core.Contracts
{
    public interface IHotelsRepository : IGenericRepository<Hotel>
    {
        Task<Hotel> GetHotelDetail(int id);
    }
}
