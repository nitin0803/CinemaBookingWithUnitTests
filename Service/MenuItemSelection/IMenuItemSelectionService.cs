using Domain.Enums;

namespace Service.MenuItemSelection;

public interface IMenuItemSelectionService
{
    void Handle(MenuItemOption menuItemOption);
}