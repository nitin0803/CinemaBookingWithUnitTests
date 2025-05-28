using Domain.Accessor;
using Domain.CinemaConsole;
using Domain.Enums;
using Domain.Utility;
using Domain.Validator;
using Microsoft.Extensions.Logging;
using Service.Screen;
using Service.SeatSelection;
using static System.String;

namespace Service.MenuItemSelection;

public class BookTicketsService(
    ICinemaConsole cinemaConsole,
    ICinemaAccessor cinemaAccessor,
    ISeatSelectionService seatSelectionService,
    IScreenService screenService,
    ILogger<CinemaAccessor> logger) : IMenuItemSelectionService
{
    public bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.BookTickets;

    public void Handle(MenuItemOption menuItemOption)
    {
        cinemaConsole.WriteLine((CinemaUtility.AppMessage.NumberOfTickets + CinemaUtility.AppMessage.Blank));

        var numberOfTicketsToBookInput = cinemaConsole.ReadNumberOfTicketsToBook();
        cinemaConsole.WriteEmptyLine();
        if (IsNullOrWhiteSpace(numberOfTicketsToBookInput)) return;

        while (!CinemaValidator.IsNumberOfTicketsToBookValid(numberOfTicketsToBookInput))
        {
            cinemaConsole.WriteLine(CinemaUtility.ValidationMessage.InvalidNumberOfTickets);
            logger.LogError(CinemaUtility.ValidationMessage.InvalidNumberOfTickets);
            numberOfTicketsToBookInput = cinemaConsole.ReadNumberOfTicketsToBook();
            cinemaConsole.WriteEmptyLine();
        }

        var numberOfTickets = Convert.ToInt32(numberOfTicketsToBookInput);
        var cinema = cinemaAccessor.GetCinema();
        
        var availableSeats = cinema.AvailableSeats;

        if (numberOfTickets > availableSeats)
        {
            cinemaConsole.WriteLine(Format(CinemaUtility.AppMessage.SeatsAvailabilityAlert, availableSeats.ToString()));
            cinemaConsole.WriteEmptyLine();
            return;
        }

        var newBookingId = seatSelectionService.ReserveSeats(numberOfTickets);
        cinemaConsole.WriteLine(Format(CinemaUtility.AppMessage.TicketsReserved, numberOfTickets, cinema.Movie));
        ShowScreen(newBookingId);

        cinemaConsole.WriteLine(CinemaUtility.AppMessage.AcceptOrNewSeatSelection);

        var newSeatPosition = cinemaConsole.ReadNewSeatPosition();
        cinemaConsole.WriteEmptyLine();
        if (HasUserAcceptedSeatSelection(newSeatPosition))
        {
            ConfirmSeats(newBookingId!);
            return;
        }

        while (!HasUserAcceptedSeatSelection(newSeatPosition))
        {
            while (!CinemaValidator.IsNewSeatPositionValid(cinema.HallLayOut.RowLayOuts, newSeatPosition!))
            {
                cinemaConsole.WriteLine(CinemaUtility.ValidationMessage.InvalidSeatingPosition);
                newSeatPosition = cinemaConsole.ReadNewSeatPosition();
                cinemaConsole.WriteEmptyLine();
            }

            seatSelectionService.FreeSeats(newBookingId!);

            newBookingId = seatSelectionService.ReserveSeats(numberOfTickets, newSeatPosition);
            ShowScreen(newBookingId);

            cinemaConsole.WriteLine(CinemaUtility.AppMessage.AcceptOrNewSeatSelection);
            newSeatPosition = cinemaConsole.ReadNewSeatPosition();
            cinemaConsole.WriteEmptyLine();
        }

        ConfirmSeats(newBookingId!);
    }

    private bool HasUserAcceptedSeatSelection(string? newSeatPosition)
    {
        return IsNullOrWhiteSpace(newSeatPosition);
    }

    private void ConfirmSeats(string newBookingId)
    {
        seatSelectionService.ConfirmSeats(newBookingId);
        logger.LogInformation($"User confirmed seats for booking id: {newBookingId}");
        cinemaConsole.WriteLine(Format(CinemaUtility.AppMessage.BookingIdConfirmed, newBookingId));
        cinemaConsole.WriteEmptyLine();
    }

    private void ShowScreen(string? newBookingId)
    {
        screenService.Show(newBookingId);
        cinemaConsole.WriteEmptyLine();
    }
}