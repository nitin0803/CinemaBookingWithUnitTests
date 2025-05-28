using Domain.Enums;
using Service.MenuItemSelection;
using Shouldly;

namespace UnitTests.Service.MenuItemSelection;

[TestClass]
public class ExitServiceTest
{
    private readonly ExitService sut = new();

    [TestMethod]
    [DataRow(MenuItemOption.BookTickets, false)]
    [DataRow(MenuItemOption.CheckBookings, false)]
    [DataRow(MenuItemOption.Exit, true)]
    public void IsResponsible_GivenMenuItemOption_ReturnsCorrectResult(
        MenuItemOption menuItemOption,
        bool expectedResult)
    {
        // Arrange & Act
        var result = sut.IsResponsible(menuItemOption);

        // Assert
        result.ShouldBe(expectedResult);
    }
}