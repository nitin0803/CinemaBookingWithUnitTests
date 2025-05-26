using Domain.Accessor;
using Domain.Enums;
using Domain.Models;
using Domain.Utility;

namespace Service.SeatSelection;

public class SeatSelectionService(ICinemaAccessor cinemaAccessor)
    : ISeatSelectionService
{
    public string ReserveSeats(int numberOfTickets, string? newSeatPosition)
    {
        var cinema = cinemaAccessor.GetCinema();
        var hallLayOut = cinema.HallLayout;

        var newBookingId = CinemaUtility.GetNewBookingId(cinema.Bookings);

        var filledSeatsCounter = 0;
        var rowLayoutsSequence = string.IsNullOrWhiteSpace(newSeatPosition)
            ? hallLayOut.RowLayOuts.Reverse()
            : GetRowLayOutsSequence(hallLayOut.RowLayOuts, newSeatPosition);

        foreach (var rowLayOut in rowLayoutsSequence)
        {
            if (filledSeatsCounter == numberOfTickets) break;

            filledSeatsCounter = !string.IsNullOrWhiteSpace(newSeatPosition)
                                 && IsNewSeatPositionBelongsToCurrentRow(rowLayOut)
                ? ReserveSeatsFromNewSeatPosition(
                    newBookingId,
                    numberOfTickets,
                    rowLayOut.Seats,
                    filledSeatsCounter,
                    newSeatPosition)
                : ReserveSeatsFromMiddle(
                    newBookingId,
                    numberOfTickets,
                    rowLayOut.Seats,
                    filledSeatsCounter);
        }

        return newBookingId;

        bool IsNewSeatPositionBelongsToCurrentRow(RowLayOut currentRow)
        {
            var newSeatPositionRowLabel = CinemaUtility.GetNewSeatPositionRowLabel(newSeatPosition);
            return currentRow.RowLabel.Equals(newSeatPositionRowLabel);
        }
    }

    public void ConfirmSeats(string newBookingId)
    {
        var cinema = cinemaAccessor.GetCinema();
        var hallLayOut = cinema.HallLayout;

        var reserveSeats = hallLayOut.RowLayOuts
            .SelectMany(r => r.Seats)
            .Where(s => string.Equals(s.BookingId, newBookingId) && s.Status == SeatStatus.Reserved)
            .ToList();

        foreach (var reserveSeat in reserveSeats)
        {
            reserveSeat.Update(SeatStatus.Confirmed, newBookingId);
        }

        var numberOfConfirmedSeats = hallLayOut.RowLayOuts
            .SelectMany(r => r.Seats)
            .Count(s => string.Equals(s.BookingId, newBookingId) && s.Status == SeatStatus.Confirmed);

        cinemaAccessor.AddBooking(new Booking(newBookingId, numberOfConfirmedSeats));
    }

    public void FreeSeats(string newBookingId)
    {
        var cinema = cinemaAccessor.GetCinema();
        var hallLayOut = cinema.HallLayout;

        var reserveSeats = hallLayOut.RowLayOuts
            .SelectMany(r => r.Seats)
            .Where(s => string.Equals(s.BookingId, newBookingId) && s.Status == SeatStatus.Reserved)
            .ToList();

        foreach (var reserveSeat in reserveSeats)
        {
            reserveSeat.Update(SeatStatus.Empty, newBookingId);
        }
    }

    private static IReadOnlyList<RowLayOut> GetRowLayOutsSequence(
        IReadOnlyList<RowLayOut> rowLayouts,
        string startSeatPosition)
    {
        var newSeatPositionRowLabel = CinemaUtility.GetNewSeatPositionRowLabel(startSeatPosition);
        var firstSequence = new List<RowLayOut>();
        var secondSequence = new List<RowLayOut>();
        foreach (var rowLayOut in rowLayouts.Reverse())
        {
            var currentRowLabel = rowLayOut.RowLabel;
            if (currentRowLabel.Equals(newSeatPositionRowLabel) ||
                currentRowLabel.CompareTo(newSeatPositionRowLabel) > -1)
            {
                firstSequence.Add(rowLayOut);
                continue;
            }

            secondSequence.Add(rowLayOut);
        }

        return firstSequence.Concat(secondSequence).ToList();
    }

    private int ReserveSeatsFromMiddle(
        string newBookingId,
        int numberOfTickets,
        IReadOnlyList<Seat> seatsInCurrentRow,
        int totalFilledSeats)
    {
        var seatsCountInCurrentRow = seatsInCurrentRow.Count;
        var seatsToFillCount = seatsInCurrentRow.Count(s => s.Status == SeatStatus.Empty);
        
        var middleSeatNumber = CinemaUtility.GetMiddleSeatNumber(seatsCountInCurrentRow, numberOfTickets);
        
        var firstSeatToReserve = GetSeatToReserve(seatsInCurrentRow, middleSeatNumber)
                                 ?? GetSeatToReserve(seatsInCurrentRow, middleSeatNumber, DirectionSide.Left);

        if (firstSeatToReserve == null) return totalFilledSeats;

        var numberOfSeatsFilledInCurrentRow = 0;
        ReserveSeat(
            firstSeatToReserve,
            newBookingId,
            ref totalFilledSeats,
            ref numberOfSeatsFilledInCurrentRow);

        if (totalFilledSeats == numberOfTickets
            || numberOfSeatsFilledInCurrentRow == seatsToFillCount)
            return totalFilledSeats;

        var rightSeatToReserve = firstSeatToReserve;
        var leftSeatToReserve = firstSeatToReserve;

        for (var i = 1; i <= seatsCountInCurrentRow; i++)
        {
            rightSeatToReserve = rightSeatToReserve == null
                ? null
                : GetSeatToReserve(seatsInCurrentRow, rightSeatToReserve.SeatNumber);

            if (rightSeatToReserve != null)
            {
                ReserveSeat(
                    rightSeatToReserve,
                    newBookingId,
                    ref totalFilledSeats,
                    ref numberOfSeatsFilledInCurrentRow);

                if (totalFilledSeats == numberOfTickets
                    || numberOfSeatsFilledInCurrentRow == seatsToFillCount)
                    break;
            }

            leftSeatToReserve = leftSeatToReserve == null
                ? null
                : GetSeatToReserve(seatsInCurrentRow, leftSeatToReserve.SeatNumber, DirectionSide.Left);
            if (leftSeatToReserve == null) continue;

            ReserveSeat(
                leftSeatToReserve,
                newBookingId,
                ref totalFilledSeats,
                ref numberOfSeatsFilledInCurrentRow);

            if (totalFilledSeats == numberOfTickets
                || numberOfSeatsFilledInCurrentRow == seatsToFillCount)
                break;
        }

        return totalFilledSeats;
    }

    private int ReserveSeatsFromNewSeatPosition(
        string newBookingId,
        int numberOfTickets,
        IReadOnlyList<Seat> seatsInCurrentRow,
        int totalFilledSeats,
        string newSeatPosition)
    {
        var newSeatPositionNumber = CinemaUtility.GetNewSeatPositionNumber(newSeatPosition);

        var seatsFromNewSeatPosition = seatsInCurrentRow
            .Where(s => s.SeatNumber >= newSeatPositionNumber)
            .ToList();

        var seatsCountToFill = seatsFromNewSeatPosition
            .Count(s => s.Status == SeatStatus.Empty);

        var numberOfSeatsFilledInCurrentRow = 0;

        var firstSeatToReserve = GetSeatToReserve(seatsInCurrentRow, newSeatPositionNumber);

        if (firstSeatToReserve == null) return totalFilledSeats;

        ReserveSeat(firstSeatToReserve, 
            newBookingId, 
            ref totalFilledSeats, 
            ref numberOfSeatsFilledInCurrentRow);

        if (totalFilledSeats == numberOfTickets || numberOfSeatsFilledInCurrentRow == seatsCountToFill)
            return totalFilledSeats;

        var nextRightSeatToReserve = firstSeatToReserve;
        for (var i = 1; i <= seatsFromNewSeatPosition.Count; i++)
        {
            var nextRightSeatNumber = nextRightSeatToReserve.SeatNumber + 1;
            nextRightSeatToReserve = GetSeatToReserve(seatsInCurrentRow, nextRightSeatNumber);

            if (nextRightSeatToReserve == null) break;

            ReserveSeat(
                nextRightSeatToReserve, 
                newBookingId, 
                ref totalFilledSeats,
                ref numberOfSeatsFilledInCurrentRow);

            if (totalFilledSeats == numberOfTickets || numberOfSeatsFilledInCurrentRow == seatsCountToFill) break;
        }

        return totalFilledSeats;
    }

    private Seat? GetSeatToReserve(
        IReadOnlyList<Seat> seatsInCurrentRow,
        int seatNumber,
        DirectionSide directionSide = DirectionSide.Right)
    {
        var seatToReserve = seatsInCurrentRow.Single(s => s.SeatNumber == seatNumber);
        while (seatToReserve.Status != SeatStatus.Empty)
        {
            var nextSeatNumberToReserve = directionSide == DirectionSide.Right
                ? seatToReserve.SeatNumber + 1
                : seatToReserve.SeatNumber - 1;

            if (HasRowLimitReached(seatsInCurrentRow, nextSeatNumberToReserve))
            {
                seatToReserve = null;
                break;
            }

            seatToReserve = seatsInCurrentRow.Single(s => s.SeatNumber == nextSeatNumberToReserve);
        }

        return seatToReserve;
    }

    private void ReserveSeat(
        Seat seatToReserve,
        string newBookingId,
        ref int totalFilledSeats,
        ref int numberOfSeatsFilledInCurrentRow)
    {
        seatToReserve.Update(SeatStatus.Reserved, newBookingId);
        totalFilledSeats++;
        numberOfSeatsFilledInCurrentRow++;
    }

    private bool HasRowLimitReached(
        IReadOnlyList<Seat> seatsInCurrentRow,
        int nextPossibleRightSeatNumberToReserve)
    {
        var leftSeatNumberLimit = seatsInCurrentRow.First().SeatNumber;
        var rightSeatNumberLimit = seatsInCurrentRow.Last().SeatNumber;

        return nextPossibleRightSeatNumberToReserve < leftSeatNumberLimit
               || nextPossibleRightSeatNumberToReserve > rightSeatNumberLimit;
    }
}