using Domain.Accessor;
using Domain.Enums;
using Domain.Models;
using Domain.Utility;
using Domain.Validator;
using Microsoft.Extensions.Logging;
using static System.String;

namespace Service.SeatSelection;

public class SeatSelectionService(ICinemaAccessor cinemaAccessor, ILogger<SeatSelectionService> logger)
    : ISeatSelectionService
{
    public string ReserveSeats(int numberOfTicketsToBook, string? newSeatPosition)
    {
        logger.LogInformation($"number of tickets to book: {numberOfTicketsToBook}");
        var cinema = cinemaAccessor.GetCinema();
        var hallRowLayOuts = cinema.HallLayOut.RowLayOuts;

        var newBookingId = CinemaUtility.GetNewBookingId(cinema.Bookings);
        logger.LogInformation($"new booking id generated: {newBookingId}");
        var rowLayOutsSequence = GetRowLayOutsSequence(hallRowLayOuts, newSeatPosition);
        var filledSeatsCounter = 0;

        foreach (var rowLayOut in rowLayOutsSequence)
        {
            if (filledSeatsCounter == numberOfTicketsToBook) break;

            filledSeatsCounter = !IsNullOrWhiteSpace(newSeatPosition)
                                 && IsNewSeatPositionBelongsToCurrentRow(rowLayOut)
                ? ReserveSeatsFromNewSeatPosition(
                    rowLayOut,
                    newBookingId,
                    numberOfTicketsToBook,
                    filledSeatsCounter,
                    newSeatPosition)
                : ReserveSeatsFromMiddle(
                    rowLayOut,
                    newBookingId,
                    numberOfTicketsToBook,
                    filledSeatsCounter);
        }

        return newBookingId;

        bool IsNewSeatPositionBelongsToCurrentRow(RowLayOut currentRow)
        {
            logger.LogInformation($"Seat position from seats to be book: {newSeatPosition}");
            var newSeatPositionRowLabel = CinemaUtility.GetSeatPositionRowLabel(newSeatPosition);
            logger.LogInformation($"Current row to fill: {currentRow.RowLabel} seats in row: {currentRow.Seats.Count}");
            return currentRow.RowLabel.Equals(newSeatPositionRowLabel);
        }
    }

    public void ConfirmSeats(string newBookingId)
    {
        var cinema = cinemaAccessor.GetCinema();
        var hallLayOut = cinema.HallLayOut;

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
        var hallLayOut = cinema.HallLayOut;

        var reserveSeats = hallLayOut.RowLayOuts
            .SelectMany(r => r.Seats)
            .Where(s => string.Equals(s.BookingId, newBookingId) && s.Status == SeatStatus.Reserved)
            .ToList();

        foreach (var reserveSeat in reserveSeats)
        {
            reserveSeat.Update(SeatStatus.Empty, newBookingId);
        }
    }

    private IReadOnlyList<RowLayOut> GetRowLayOutsSequence(
        IReadOnlyList<RowLayOut> rowLayouts,
        string? newSeatPosition)
    {
        if (IsNullOrWhiteSpace(newSeatPosition)) return rowLayouts.Reverse().ToList();

        if (!CinemaValidator.IsNewSeatPositionValid(rowLayouts, newSeatPosition))
        {
            var exceptionMessage =
                Format($"{CinemaUtility.ExceptionMessage.SeatingPositionValueNotCorrect}", newSeatPosition);
            logger.LogCritical(exceptionMessage);
            throw new Exception(exceptionMessage);
        }

        var newSeatPositionRowLabel = CinemaUtility.GetSeatPositionRowLabel(newSeatPosition);
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
        RowLayOut rowLayOut,
        string? newBookingId,
        int numberOfTicketsBook,
        int totalFilledSeats)
    {
        var seatsInCurrentRow = rowLayOut.Seats;
        var seatsCountInCurrentRow = seatsInCurrentRow.Count;
        var seatsCountToFillInCurrentRow = seatsInCurrentRow.Count(s => s.Status == SeatStatus.Empty);

        var middleSeatNumber = CinemaUtility.GetMiddleSeatNumber(seatsCountInCurrentRow, numberOfTicketsBook);

        var firstSeatToReserve = GetSeatToReserve(seatsInCurrentRow, middleSeatNumber)
                                 ?? GetSeatToReserve(seatsInCurrentRow, middleSeatNumber, DirectionSide.Left);

        if (firstSeatToReserve == null) return totalFilledSeats;

        var numberOfSeatsFilledInCurrentRow = 0;

        logger.LogInformation($"{nameof(ReserveSeatsFromMiddle)}: Reserving seats in row: {rowLayOut.RowLabel}");
        ReserveSeat(
            firstSeatToReserve,
            newBookingId,
            ref totalFilledSeats,
            ref numberOfSeatsFilledInCurrentRow);

        if (totalFilledSeats == numberOfTicketsBook
            || numberOfSeatsFilledInCurrentRow == seatsCountToFillInCurrentRow)
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

                if (totalFilledSeats == numberOfTicketsBook
                    || numberOfSeatsFilledInCurrentRow == seatsCountToFillInCurrentRow)
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

            if (totalFilledSeats == numberOfTicketsBook
                || numberOfSeatsFilledInCurrentRow == seatsCountToFillInCurrentRow)
                break;
        }

        return totalFilledSeats;
    }

    private int ReserveSeatsFromNewSeatPosition(
        RowLayOut rowLayOut,
        string? newBookingId,
        int numberOfTicketsToBook,
        int totalFilledSeats,
        string newSeatPosition)
    {
        logger.LogInformation($"{nameof(ReserveSeatsFromNewSeatPosition)}: Reserving seats in row: {rowLayOut.RowLabel}");
        var seatsInCurrentRow = rowLayOut.Seats;
        var newSeatPositionNumber = CinemaUtility.GetSeatPositionNumber(newSeatPosition);
        var seatsFromNewSeatPosition = seatsInCurrentRow
            .Where(s => s.SeatNumber >= newSeatPositionNumber)
            .ToList();

        var seatsCountToFillInCurrentRow = seatsFromNewSeatPosition
            .Count(s => s.Status == SeatStatus.Empty);

        var numberOfSeatsFilledInCurrentRow = 0;

        var firstSeatToReserve = GetSeatToReserve(seatsInCurrentRow, newSeatPositionNumber);

        if (firstSeatToReserve == null) return totalFilledSeats;

        ReserveSeat(firstSeatToReserve,
            newBookingId,
            ref totalFilledSeats,
            ref numberOfSeatsFilledInCurrentRow);

        if (totalFilledSeats == numberOfTicketsToBook ||
            numberOfSeatsFilledInCurrentRow == seatsCountToFillInCurrentRow)
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

            if (totalFilledSeats == numberOfTicketsToBook ||
                numberOfSeatsFilledInCurrentRow == seatsCountToFillInCurrentRow) break;
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
        string? newBookingId,
        ref int totalFilledSeats,
        ref int numberOfSeatsFilledInCurrentRow)
    {
        seatToReserve.Update(SeatStatus.Reserved, newBookingId);
        totalFilledSeats++;
        numberOfSeatsFilledInCurrentRow++;
        logger.LogInformation($"Reserved seat number: {seatToReserve.SeatNumber}");
        logger.LogInformation($"Total filled seats till now: {totalFilledSeats}");
        logger.LogInformation($"Number of seats filled in current row: {numberOfSeatsFilledInCurrentRow}");
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