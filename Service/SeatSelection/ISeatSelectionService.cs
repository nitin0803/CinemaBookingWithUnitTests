namespace Service.SeatSelection;

public interface ISeatSelectionService
{
    string? ReserveSeats(int numberOfTicketsToBook, string? newSeatPosition = null);
    void ConfirmSeats(string bookingId);
    void FreeSeats(string newBookingId);
}