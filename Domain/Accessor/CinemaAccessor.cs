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
        GetCinema().AddBooking(booking);
    }

    public Booking? TryGetBooking(string bookingId)
    {
        try
        {
            return GetCinema().Bookings.SingleOrDefault(b => b.BookingId == bookingId); 
        }
        catch (Exception)
        {
            Console.WriteLine(CinemaUtility.ExceptionMessage.DuplicateBookingsFound, bookingId);
            throw;
        }
    }
}