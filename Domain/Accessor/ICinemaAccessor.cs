using Domain.Models;

namespace Domain.Accessor;

public interface ICinemaAccessor
{
    Cinema CreateCinema(string movie, int rows, int seatsPerRow);
    Cinema GetCinema();
    void AddBooking(Booking booking);
    Booking? TryGetBooking(string? bookingId);
}