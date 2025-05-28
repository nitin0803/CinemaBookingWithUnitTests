using Domain.Accessor;
using Domain.CinemaConsole;
using Domain.Enums;
using Domain.Utility;
using Domain.Validator;
using Microsoft.Extensions.Logging;
using Service.Screen;
using static System.String;

namespace Service.MenuItemSelection;

public class CheckBookingsService(
    ICinemaAccessor cinemaAccessor,
    IScreenService screenService,
    ICinemaConsole cinemaConsole,
    ILogger<CinemaAccessor> logger)
    : IMenuItemSelectionService
{
    public bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.CheckBookings;

    public void Handle(MenuItemOption menuItemOption)
    {
        Console.WriteLine(CinemaUtility.AppMessage.BookingId + CinemaUtility.AppMessage.Blank);
        var bookingId = cinemaConsole.ReadBookingId();
        Console.WriteLine();
        while (!IsNullOrWhiteSpace(bookingId))
        {
            logger.LogInformation($"Checking booking for booking id: {bookingId}");
            ShowBooking(bookingId);
            Console.WriteLine();
            Console.WriteLine((CinemaUtility.AppMessage.BookingId + CinemaUtility.AppMessage.Blank));
            bookingId = cinemaConsole.ReadBookingId();
            Console.WriteLine();
        }
    }

    private void ShowBooking(string? bookingId)
    {
        while (!CinemaValidator.IsBookingIdValid(bookingId))
        {
            Console.WriteLine(CinemaUtility.ValidationMessage.InvalidBookingIdFormat);
            logger.LogError(CinemaUtility.ValidationMessage.InvalidBookingIdFormat);
            bookingId = cinemaConsole.ReadBookingId();
            Console.WriteLine();
        }

        var booking = cinemaAccessor.TryGetBooking(bookingId);
        if (booking == null)
        {
            Console.WriteLine(CinemaUtility.ValidationMessage.NoBookingFound, bookingId);
            logger.LogError(Format(CinemaUtility.ValidationMessage.NoBookingFound, bookingId));
            return;
        }

        screenService.Show(bookingId);
    }
}