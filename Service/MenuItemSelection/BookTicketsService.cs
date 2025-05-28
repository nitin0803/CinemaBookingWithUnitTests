using Domain.Accessor;
using Domain.CinemaConsole;
using Domain.Enums;
using Domain.Utility;
using Domain.Validator;
using Service.Screen;
using Service.SeatSelection;

namespace Service.MenuItemSelection;

public class BookTicketsService(
    ICinemaConsole cinemaConsole,
    ICinemaAccessor cinemaAccessor,
    ISeatSelectionService seatSelectionService,
    IScreenService screenService) : IMenuItemSelectionService
{
    public bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.BookTickets;

    public void Handle(MenuItemOption menuItemOption)
    {
        cinemaConsole.WriteLine((CinemaUtility.AppMessage.NumberOfTickets + CinemaUtility.AppMessage.Blank));

        var numberOfTicketsToBookInput = cinemaConsole.EnterNumberOfTicketsToBook();
        cinemaConsole.WriteEmptyLine();
        if (string.IsNullOrWhiteSpace(numberOfTicketsToBookInput)) return;

        while (!CinemaValidator.IsNumberOfTicketsToBookValid(numberOfTicketsToBookInput))
        {
            cinemaConsole.WriteLine(CinemaUtility.ValidationMessage.InvalidNumberOfTickets);
            numberOfTicketsToBookInput = cinemaConsole.EnterNumberOfTicketsToBook();
            cinemaConsole.WriteEmptyLine();
        }

        var numberOfTickets = Convert.ToInt32(numberOfTicketsToBookInput);
        var cinema = cinemaAccessor.GetCinema();
        
        var availableSeats = cinema.AvailableSeats;

        if (numberOfTickets > availableSeats)
        {
            cinemaConsole.WriteLine(String.Format(CinemaUtility.AppMessage.SeatsAvailabilityAlert, availableSeats.ToString()));
            cinemaConsole.WriteEmptyLine();
            return;
        }

        var newBookingId = seatSelectionService.ReserveSeats(numberOfTickets);
        cinemaConsole.WriteLine(String.Format(CinemaUtility.AppMessage.TicketsReserved, numberOfTickets, cinema.Movie));
        ShowScreen(newBookingId);

        cinemaConsole.WriteLine(CinemaUtility.AppMessage.AcceptOrNewSeatSelection);

        var newSeatPosition = cinemaConsole.EnterNewSeatPosition();
        cinemaConsole.WriteEmptyLine();
        if (HasUserAcceptedSeatSelection(newSeatPosition))
        {
            ConfirmSeats(newBookingId);
            return;
        }

        while (!HasUserAcceptedSeatSelection(newSeatPosition))
        {
            while (!CinemaValidator.IsNewSeatPositionValid(cinema.HallLayOut.RowLayOuts, newSeatPosition!))
            {
                cinemaConsole.WriteLine(CinemaUtility.ValidationMessage.InvalidSeatingPosition);
                newSeatPosition = cinemaConsole.EnterNewSeatPosition();
                cinemaConsole.WriteEmptyLine();
            }

            seatSelectionService.FreeSeats(newBookingId);

            newBookingId = seatSelectionService.ReserveSeats(numberOfTickets, newSeatPosition);
            ShowScreen(newBookingId);

            cinemaConsole.WriteLine(CinemaUtility.AppMessage.AcceptOrNewSeatSelection);
            newSeatPosition = cinemaConsole.EnterNewSeatPosition();
            cinemaConsole.WriteEmptyLine();
        }

        ConfirmSeats(newBookingId);
    }

    private bool HasUserAcceptedSeatSelection(string? newSeatPosition)
    {
        return string.IsNullOrWhiteSpace(newSeatPosition);
    }

    private void ConfirmSeats(string newBookingId)
    {
        seatSelectionService.ConfirmSeats(newBookingId);
        cinemaConsole.WriteLine(string.Format(CinemaUtility.AppMessage.BookingIdConfirmed, newBookingId));
        cinemaConsole.WriteEmptyLine();
    }

    private void ShowScreen(string newBookingId)
    {
        screenService.Show(newBookingId);
        cinemaConsole.WriteEmptyLine();
    }
}