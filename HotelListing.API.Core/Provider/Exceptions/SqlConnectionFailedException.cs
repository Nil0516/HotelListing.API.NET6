namespace HotelListing.API.Core.Provider.Exceptions
{
    public class SqlConnectionFailedException : Exception
    {
        public SqlConnectionFailedException()
        {
        }

        public SqlConnectionFailedException(string message) : base(message)
        {
        }
    }
}
