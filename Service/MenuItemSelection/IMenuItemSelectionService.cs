using Domain.Enums;

namespace Service.MenuItemSelection;

public interface IMenuItemSelectionService
{
    bool IsResponsible(MenuItemOption menuItemOption);
    void Handle(MenuItemOption menuItemOption);
}