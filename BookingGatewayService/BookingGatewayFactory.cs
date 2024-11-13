using BookingGatewayRepository;
using System;

namespace BookingGatewayService
{
    /// <summary>
    /// The class should be protected from inheritance! Done!
    /// Protected with sealed
    /// </summary>
    public sealed class BookingGatewayFactory
    {

        /// <summary>
        /// The method creates an instance of IBookingGateway Done!
        /// </summary>
        /// <param name="repository">The database repository for transactions</param>
        /// <returns>An instance of IBookingGateway</returns>
        public static IBookingGateway CreateGateway(IDBRepository repository)
        {
            // If the repository is null, an error is thrown.
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository), "Repository cannot be null.");
            }

            // It returns the object that implements the IBookingGateway interface.
            return new BookingGateway(repository);
        }

        /// <summary>
        /// The method is deprecated, but the developer can use it!
        /// </summary>
        /// <returns>An instance of BookingGatewayFactory</returns>
        public static object CreateObject()
        {
            return new BookingGatewayFactory();
        }

        /// <summary>
        /// The method is deprecated! Developer cannot use it! If used, it should result in a compilation error!
        /// </summary>
        /// <returns>New object instance</returns>
        [Obsolete("This method is deprecated and should not be used.", true)]
        public static object NewObject()
        {
            // Throws exception if someone tries to use this method
            throw new NotImplementedException("This method is no longer supported and cannot be used.");

        }
    }
}
