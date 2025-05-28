using Domain.Enums;
using Domain.Utility;

namespace Service.MenuItemSelection;

public class ExitService : IMenuItemSelectionService
{
    public bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.Exit;

    public void Handle(MenuItemOption menuItemOption)
    {
        Console.WriteLine(CinemaUtility.AppMessage.ThankYou);
        Console.WriteLine();
    }
}