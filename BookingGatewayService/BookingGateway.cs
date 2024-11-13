using BookingGatewayRepository;
using BookingGatewayRepository.Model;
using BookingGatewayService.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BookingGatewayService
{
    //Sealed to protect from inheritance
    public sealed class BookingGateway : IBookingGateway
    {
        private readonly object _bookingLock = new object(); // Operation lock
        private readonly object _statusLock = new object();  // State read lock
        private bool _isBookingInProgress;
        private bool _isReadInProgress;
        private Thread _currentThread;

        public IDBRepository DBRepository { get; set; }

        public BookingGateway(IDBRepository dbRepository)
        {
            DBRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));
            _isBookingInProgress = false;
            _isReadInProgress = false;
        }

        // Start process
        public void StartBooking()
        {
            lock (_bookingLock)
            {   //Maybe Thread.VolatileRead?
                if (_isBookingInProgress)
                    throw new BookingInProgressException();
                //Maybe Thread.VolatileRead?
                if (_isReadInProgress)
                    throw new ReadOperationInProgressException();

                _isBookingInProgress = true;
                _currentThread = Thread.CurrentThread; // The thread that initiates the reservation.
            }
        }

        // The reservation record creation process.
        public void Booking(string uniqueReference, decimal amount, string transactionTitle, string srcAccountNo, string destAccountNo)
        {
            if (!_isBookingInProgress)
                throw new NoStartBookingException();

            
            var transactionData = new TransactionData
            {
                UniqueRef = uniqueReference,
                Amount = amount,
                TransactionTitle = transactionTitle,
                SourceAccountNo = srcAccountNo,
                DestAccountNo = destAccountNo
            };

            DBRepository.SaveBooking(transactionData);
        }

        // The process of completing the reservation.
        public void EndBooking()
        {
            lock (_bookingLock)
            {
                if (!_isBookingInProgress)
                    throw new NoStartBookingException();

                _isBookingInProgress = false;
            }
        }

        public IList<TransactionStatus> GetBookingStatuses(IList<string> uniqueTransactionRefs)
        {
            lock (_statusLock)
            {
                // Reading should be prevented while another operation is being performed.
                if (_isBookingInProgress && Thread.CurrentThread != _currentThread)
                    throw new BookingInProgressException();

                // Nullcheck for uniqueTransactionRefs if empty return new empty list
                if (uniqueTransactionRefs == null || uniqueTransactionRefs.Count == 0)
                    return new List<TransactionStatus>();

                _isReadInProgress = true;
                var statuses = DBRepository.GetStatuses(uniqueTransactionRefs.ToArray());
                _isReadInProgress = false;

                return statuses.ToList() ?? new List<TransactionStatus>();
            }
        }
    }
}
