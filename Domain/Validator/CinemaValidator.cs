using System.Text.RegularExpressions;
using Domain.Models;
using Domain.Utility;

namespace Domain.Validator;

public static class CinemaValidator
{
    public static bool AreCinemaDetailsValid(string? cinemaDetails)
    {
        if (string.IsNullOrWhiteSpace(cinemaDetails)) return false;

        var cinemaValues = cinemaDetails.Split(" ");
        if (cinemaValues.Length != 3)
        {
            Console.WriteLine(CinemaUtility.ValidationMessage.InvalidMovieDetails);
            return false;
        }

        if (cinemaValues[0].Length > 50)
        {
            Console.WriteLine(CinemaUtility.ValidationMessage.MovieNameExceed);
            return false;
        }

        if (!AreRowsValid(cinemaValues)) return false;

        if (!AreSeatsPerRowValid(cinemaValues)) return false;

        return true;
    }

    public static bool IsBookingIdValid(string? bookingId)
    {
        return bookingId != null && Regex.IsMatch(bookingId, CinemaUtility.RegexPattern.BookingId);
    }

    public static bool IsNewSeatPositionValid(IReadOnlyList<RowLayOut> rowLayouts, string newSeatPosition)
    {
        if (!Regex.IsMatch(newSeatPosition, CinemaUtility.RegexPattern.SeatPosition)) return false;

        var newSeatPositionRowLabel = CinemaUtility.GetSeatPositionRowLabel(newSeatPosition);

        var allLabels = rowLayouts.Select(r => r.RowLabel).ToList();
        if (!allLabels.Contains(newSeatPositionRowLabel)) return false;

        var newSeatPositionNumber = CinemaUtility.GetSeatPositionNumber(newSeatPosition);
        var allSeatNumbersInRow = rowLayouts
            .First().Seats
            .Select(s => s.SeatNumber).ToList();
        
        if (!allSeatNumbersInRow.Contains(newSeatPositionNumber)) return false;
        
        return true;
    }

    public static bool IsNumberOfTicketsToBookValid(string? numberOfTicketsInput)
    {
        if (!int.TryParse(numberOfTicketsInput, out int numberOfTickets))
        {
            return false;
        }

        if (numberOfTickets < 1)
        {
            return false;
        }

        return true;
    }

    private static bool AreSeatsPerRowValid(string[] inputArray)
    {
        if (!int.TryParse(inputArray[2], out var seatsPerRow))
        {
            Console.WriteLine(CinemaUtility.ValidationMessage.InvalidSeatsPerRow);
            return false;
        }

        if (seatsPerRow is < 1 or > 50)
        {
            Console.WriteLine(CinemaUtility.ValidationMessage.SeatsPerRowRangeExceed);
            return false;
        }

        return true;
    }

    private static bool AreRowsValid(string[] inputArray)
    {
        if (!int.TryParse(inputArray[1], out var rows))
        {
            Console.WriteLine(CinemaUtility.ValidationMessage.InvalidRow);
            return false;
        }

        if (rows is < 1 or > 26)
        {
            Console.WriteLine(CinemaUtility.ValidationMessage.RowRangeExceed);
            return false;
        }

        return true;
    }
}