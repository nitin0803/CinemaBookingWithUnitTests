using Domain.Models;
using Domain.Utility;
using Microsoft.Extensions.Logging;

namespace Domain.Accessor;

public class CinemaAccessor(ILogger<CinemaAccessor> logger) : ICinemaAccessor
{
    public Cinema CreateCinema(string movie, int rows, int seatsPerRow)
    {
        return Cinema.Create(movie, rows, seatsPerRow);
    }

    public Cinema GetCinema()
    {
        try
        {
            return Cinema.GetCinema();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception occurred as no Cinema available!");
            var exceptionMessage = $"Exception occurred: {e.Message}";
            logger.LogCritical(exceptionMessage);
            throw;
        }
    }

    public void AddBooking(Booking booking)
    {
        var cinema = GetCinema();

        var isBookingAlreadyExist = cinema.Bookings.Any(b => b.BookingId.Equals(booking.BookingId));

        if (isBookingAlreadyExist)
        {
            var exceptionMessage =
                string.Format($"{CinemaUtility.ExceptionMessage.BookingAlreadyExist}", booking.BookingId);
            logger.LogCritical(exceptionMessage);
            throw new Exception(exceptionMessage);
        }

        cinema.AddBooking(booking);
    }

    public Booking? TryGetBooking(string? bookingId)
    {
        return GetCinema().Bookings.SingleOrDefault(b => b.BookingId == bookingId);
    }
}