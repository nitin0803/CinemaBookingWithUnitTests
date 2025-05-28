using Domain.Models;

namespace Domain.Utility;

public static class CinemaUtility
{
    public struct RegexPattern
    {
        public const string BookingId = @"^GIC\d{4}$";
        public const string SeatPosition = @"^[A-Za-z]\d+$";
    }

    public struct AppMessage
    {
        public const string DefineCinema =
            "Please define movie title and seating map in [Title] [Row] [SeatsPerRow] format:";

        public const string Welcome = "Welcome to GIC Cinemas";
        public const string EnterSelection = "Please enter your selection:";

        public const string Blank = "or enter blank to go back to main menu:";
        public const string NumberOfTickets = "Enter number of tickets to book, ";
        public const string BookingId = "Enter booking id, ";
        public const string ThankYou = "Thank you for using GIC Cinames system. Bye!";

        public const string AcceptOrNewSeatSelection =
            "Enter blank to accept seat selection, or enter new seating position:";

        public const string TicketsReserved = "Successfully reserved {0} {1} tickets.";
        public const string BookingIdConfirmed = "Booking id: {0} confirmed.";
        public const string SeatsAvailabilityAlert = "Sorry, there are only {0} seats available.";
    }

    public struct MenuItem
    {
        public const string BookTickets = "[1] Book tickets for {0} ({1} seats available)";
        public const string CheckBookings = "[2] Check bookings";
        public const string Exit = "[3] Exit";
    }

    public struct ValidationMessage
    {
        public const string InvalidSelection = "Input selection is not correct, please try again!";
        public const string InvalidMovieDetails = "Entered movie details are not in correct format!";
        public const string MovieNameExceed = "Please, enter movie name less than 50 characters";
        public const string InvalidRow = "Please, enter row as positive integer value";
        public const string RowRangeExceed = "Please, enter rows between 1 and 26, inclusive";
        public const string InvalidSeatsPerRow = "Please, enter seats per row as positive integer value";
        public const string SeatsPerRowRangeExceed = "Please, enter seats per row between 1 and 50, inclusive";
        public const string InvalidNumberOfTickets = "Please, enter number of tickets as positive integer value";
        public const string InvalidSeatingPosition = "New seating position is not valid! Please try again:";
        public const string InvalidBookingIdFormat = "Entered booking id is not in correct format, please try again!";
        public const string NoBookingFound = "Sorry, no booking Found for entered booking id: {0}";
    }

    public struct ExceptionMessage
    {
        public const string NoCinemaFound =
            "No Cinema found!";
        
        public const string SeatingPositionValueNotCorrect =
            "Seating position value {0} is not correct!";

        public const string BookingAlreadyExist =
            "Exception occurred as booking already exist for booking id: {0} !";
    }

    public static char GetSeatPositionRowLabel(string seatPosition)
    {
        return seatPosition.Substring(0, 1)[0];
    }

    public static int GetSeatPositionNumber(string newSeatPosition) => Convert.ToInt32(newSeatPosition.Substring(1));

    public static string GetNewBookingId(IReadOnlyList<Booking> currentBookings)
    {
        var lastBookingNumber = currentBookings.Count == 0
            ? 0
            : Convert.ToInt32(currentBookings.Last().BookingId.Substring(3));

        return "GIC" + (lastBookingNumber + 1).ToString("D4");
    }

    public static int GetMiddleSeatNumber(int seatsCountInCurrentRow, int numberOfTicketsBook)
    {
        if (seatsCountInCurrentRow % 2 != 0) return seatsCountInCurrentRow / 2 + 1;

        var medianSeatNumber = seatsCountInCurrentRow / 2;
        
        return numberOfTicketsBook % 2 == 0 ? medianSeatNumber : medianSeatNumber + 1;
    }
}