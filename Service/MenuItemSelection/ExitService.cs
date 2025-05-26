using Domain.Enums;
using Domain.Utility;

namespace Service.MenuItemSelection;

public class ExitService : IMenuItemSelectionService
{
    public void Handle(MenuItemOption menuItemOption)
    {
        if (!IsResponsible(menuItemOption)) return;
        Console.WriteLine((string?)CinemaUtility.AppMessage.ThankYou);
        Console.WriteLine();
    }

    private static bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.Exit;
}