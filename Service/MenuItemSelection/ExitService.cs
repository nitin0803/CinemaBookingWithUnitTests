using Domain.Enums;
using Domain.Utility;

namespace Service.MenuItemSelection;

public class ExitService : IMenuItemSelectionService
{
    public bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.Exit;

    public void Handle(MenuItemOption menuItemOption)
    {
        Console.WriteLine((string?)CinemaUtility.AppMessage.ThankYou);
        Console.WriteLine();
    }
}