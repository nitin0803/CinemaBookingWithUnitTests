using System.Text.RegularExpressions;
using Domain.Accessor;
using Domain.Enums;
using Domain.Utility;
using Service.Screen;

namespace Service.MenuItemSelection;

public class CheckBookingsService(ICinemaAccessor cinemaAccessor, IScreenService screenService) : IMenuItemSelectionService
{
    private const string BookingIdPattern = @"^GIC\d{4}$";

    public void Handle(MenuItemOption menuItemOption)
    {
        if (!IsResponsible(menuItemOption)) return;
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

    private static bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.CheckBookings;

    private void ShowBooking(string bookingId)
    {
        while (string.IsNullOrWhiteSpace(bookingId) || !Regex.IsMatch(bookingId, BookingIdPattern))
        {
            Console.WriteLine((string?)CinemaUtility.ValidationMessage.InvalidBookingIdFormat);
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