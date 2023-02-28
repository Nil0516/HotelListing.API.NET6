namespace HotelListing.API.Provider.Exceptions
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
