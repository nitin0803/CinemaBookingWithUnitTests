using Domain.Accessor;
using Domain.CinemaConsole;
using Domain.Enums;
using Domain.Utility;
using Domain.Validator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.MenuItemSelection;

namespace App.Controller;

public class CinemaController(
    ICinemaConsole cinemaConsole,
    ICinemaAccessor cinemaAccessor,
    IEnumerable<IMenuItemSelectionService> menuItemSelectionServices,
    ILogger<CinemaAccessor> logger)
    : ICinemaController
{
    public void StartCinemaApplication()
    {
        cinemaConsole.WriteLine(CinemaUtility.AppMessage.DefineCinema);
        var inputString = cinemaConsole.DefineCinema();
        cinemaConsole.WriteEmptyLine();

        while (!CinemaValidator.AreCinemaDetailsValid(inputString))
        {
            cinemaConsole.WriteLine(CinemaUtility.AppMessage.DefineCinema);
            inputString = cinemaConsole.DefineCinema();
            cinemaConsole.WriteEmptyLine();
        }

        var inputArray = inputString!.Split(" ");
        var rows = int.Parse(inputArray[1]);
        var seatsPerRow = int.Parse(inputArray[2]);

        var cinema = cinemaAccessor.CreateCinema(inputArray[0], rows, seatsPerRow);
        logger.Log(LogLevel.Information, $"Cinema created as Movie name: {cinema.Movie}, rows: {cinema.TotalRows}, seatsPerRow: {cinema.SeatsPerRow}");

        var menuItemOption = MenuItemOption.None;
        while (menuItemOption != MenuItemOption.Exit)
        {
            cinemaConsole.WriteLine(CinemaUtility.AppMessage.Welcome);
            var bookTicketsMessage =
                string.Format(CinemaUtility.MenuItem.BookTickets, cinema.Movie, cinema.AvailableSeats); 
            cinemaConsole.WriteLine(bookTicketsMessage);
            cinemaConsole.WriteLine(CinemaUtility.MenuItem.CheckBookings);
            cinemaConsole.WriteLine(CinemaUtility.MenuItem.Exit);
            cinemaConsole.WriteLine(CinemaUtility.AppMessage.EnterSelection);

            var menuItemSelection = cinemaConsole.SelectMenuItem();
            cinemaConsole.WriteEmptyLine();

            if (!Enum.TryParse(menuItemSelection, out menuItemOption)
                || !Enum.GetValues<MenuItemOption>().Contains(menuItemOption))
            {
                cinemaConsole.WriteLine(CinemaUtility.ValidationMessage.InvalidSelection);
                cinemaConsole.WriteEmptyLine();
                menuItemOption = MenuItemOption.None;
                continue;
            }

            var menuItemSelectionService = menuItemSelectionServices
                .Single(s => s.IsResponsible(menuItemOption));

            menuItemSelectionService.Handle(menuItemOption);
        }
    }
}