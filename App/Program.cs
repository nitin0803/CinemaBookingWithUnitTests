using Domain.Accessor;
using Domain.Enums;
using Domain.Utility;
using Microsoft.Extensions.DependencyInjection;
using Service;
using Service.MenuItemSelection;

var serviceProvider = ServiceModule.RegisterDependencies();

Console.WriteLine(CinemaUtility.AppMessage.DefineCinema);

var inputString = Console.ReadLine();
Console.WriteLine();

while (!CinemaUtility.AreCinemaDetailsValid(inputString))
{
    Console.WriteLine(CinemaUtility.AppMessage.DefineCinema);
    inputString = Console.ReadLine();
    Console.WriteLine();
}

var inputArray = inputString!.Split(" ");
var rows = int.Parse(inputArray[1]);
var seatsPerRow = int.Parse(inputArray[2]);

var cinemaService = serviceProvider.GetRequiredService<ICinemaAccessor>();
var cinema = cinemaService.CreateCinema(inputArray[0], rows, seatsPerRow);

MenuItemOption menuItemOption = MenuItemOption.None;
while (menuItemOption != MenuItemOption.Exit)
{
    Console.WriteLine(CinemaUtility.AppMessage.Welcome);
    Console.WriteLine(CinemaUtility.MenuItem.BookTickets, cinema.Movie, cinema.AvailableSeats);
    Console.WriteLine(CinemaUtility.MenuItem.CheckBookings);
    Console.WriteLine(CinemaUtility.MenuItem.Exit);
    Console.WriteLine(CinemaUtility.AppMessage.EnterSelection);

    var menuItemSelection = Console.ReadLine();
    Console.WriteLine();

    if (!Enum.TryParse(menuItemSelection, out menuItemOption)
        || !Enum.GetValues<MenuItemOption>().Contains(menuItemOption))
    {
        Console.WriteLine(CinemaUtility.ValidationMessage.InvalidSelection);
        Console.WriteLine();
        menuItemOption = MenuItemOption.None;
        continue;
    }

    var userSelectionServices = serviceProvider.GetServices<IMenuItemSelectionService>();
    foreach (var userSelectionService in userSelectionServices)
    {
        userSelectionService.Handle(menuItemOption);
    }
}