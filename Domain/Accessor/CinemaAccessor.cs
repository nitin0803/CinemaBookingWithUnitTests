using Domain.Models;
using Domain.Utility;

namespace Domain.Accessor;

public class CinemaAccessor : ICinemaAccessor
{
    public Cinema CreateCinema(string movie, int rows, int seatsPerRow)
    {
        return Cinema.Create(movie, rows, seatsPerRow);
    }

    public Cinema GetCinema()
    {
        return Cinema.GetCinema();
    }

    public void AddBooking(Booking booking)
    {
        var cinema = GetCinema();

        var isBookingAlreadyExist = cinema.Bookings.Any(b => b.BookingId.Equals(booking.BookingId));

        if (isBookingAlreadyExist)
        {
            var exceptionMessage = string.Format($"{CinemaUtility.ExceptionMessage.BookingAlreadyExist}", booking.BookingId);
            throw new Exception(exceptionMessage);
        }

        cinema.AddBooking(booking);
    }

    public Booking? TryGetBooking(string bookingId)
    {
        return GetCinema().Bookings.SingleOrDefault(b => b.BookingId == bookingId);
    }
}