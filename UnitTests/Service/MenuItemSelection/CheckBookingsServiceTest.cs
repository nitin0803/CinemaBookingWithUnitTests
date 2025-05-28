using Domain.Accessor;
using Domain.CinemaConsole;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Service.MenuItemSelection;
using Service.Screen;
using Shouldly;

namespace UnitTests.Service.MenuItemSelection;

[TestClass]
public class CheckBookingsServiceTest
{
    private readonly Mock<ICinemaAccessor> _cinemaAccessorMock = new();
    private readonly Mock<IScreenService> _screenServiceMock = new();
    private readonly Mock<ILogger<CinemaAccessor>> loggerMock =new();
    private readonly Mock<ICinemaConsole> consoleMock =new();
    private readonly CheckBookingsService _sut;

    public CheckBookingsServiceTest()
    {
        _sut = new CheckBookingsService(
            _cinemaAccessorMock.Object,
            _screenServiceMock.Object,
            consoleMock.Object,
            loggerMock.Object);
    }

    [TestMethod]
    [DataRow(MenuItemOption.BookTickets, false)]
    [DataRow(MenuItemOption.CheckBookings, true)]
    [DataRow(MenuItemOption.Exit, false)]
    public void IsResponsible_GivenMenuItemOption_ReturnsCorrectResult(
        MenuItemOption menuItemOption,
        bool expectedResult)
    {
        // Arrange & Act
        var result = _sut.IsResponsible(menuItemOption);

        // Assert
        result.ShouldBe(expectedResult);
    }
}