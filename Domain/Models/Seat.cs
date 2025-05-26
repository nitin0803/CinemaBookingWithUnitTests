using Domain.Enums;

namespace Domain.Models;

public class Seat(int seatNumber)
{
    public int SeatNumber { get; } = seatNumber;
    public SeatStatus Status { get; private set; } = SeatStatus.Empty;
    public string? BookingId { get; private set; }

    public void Update(SeatStatus seatStatus, string bookingId)
    {
        Status = seatStatus;
        BookingId = bookingId;
    }
}