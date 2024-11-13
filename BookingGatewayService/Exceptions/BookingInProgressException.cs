using System;

namespace BookingGatewayService.Exceptions
{
    //Sealed to protect inheritance
    public sealed class BookingInProgressException : Exception
    {
    }
}
