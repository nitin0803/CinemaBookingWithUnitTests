using System.Text.RegularExpressions;
using Domain;
using Domain.Accessor;
using Domain.Enums;
using Domain.Utility;
using Domain.Validator;
using Service.Screen;

namespace Service.MenuItemSelection;

public class CheckBookingsService(
    ICinemaAccessor cinemaAccessor,
    IScreenService screenService)
    : IMenuItemSelectionService
{
    public bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.CheckBookings;

    public void Handle(MenuItemOption menuItemOption)
    {
        Console.WriteLine((string?)(CinemaUtility.AppMessage.BookingId + CinemaUtility.AppMessage.Blank));
        var bookingId = Console.ReadLine();
        Console.WriteLine();
        while (!string.IsNullOrWhiteSpace(bookingId))
        {
            ShowBooking(bookingId);
            Console.WriteLine();
            Console.WriteLine((string?)(CinemaUtility.AppMessage.BookingId + CinemaUtility.AppMessage.Blank));
            bookingId = Console.ReadLine();
            Console.WriteLine();
        }
    }

    private void ShowBooking(string bookingId)
    {
        while (!CinemaValidator.IsBookingIdValid(bookingId))
        {
            Console.WriteLine(CinemaUtility.ValidationMessage.InvalidBookingIdFormat);
            bookingId = Console.ReadLine() ?? string.Empty;
            Console.WriteLine();
        }

        var booking = cinemaAccessor.TryGetBooking(bookingId);
        if (booking == null)
        {
            Console.WriteLine(CinemaUtility.AppMessage.NoBookingFound, bookingId);
            return;
        }

        screenService.Show(bookingId);
    }
}