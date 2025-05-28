using Domain.Accessor;
using Domain.CinemaConsole;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Service.MenuItemSelection;
using Service.Screen;
using Service.SeatSelection;
using Shouldly;

namespace UnitTests.Service.MenuItemSelection;

[TestClass]
public class BookTicketsServiceTest
{
    private readonly Mock<ICinemaConsole> cinemaConsoleMock = new();
    private readonly Mock<ICinemaAccessor> cinemaAccessorMock = new();
    private readonly Mock<ISeatSelectionService> seatSelectionServiceMock = new();
    private readonly Mock<IScreenService> screenServiceMock = new();
    private readonly Mock<ILogger<CinemaAccessor>> loggerMock =new();
    private readonly BookTicketsService sut;

    public BookTicketsServiceTest()
    {
        sut = new BookTicketsService(
            cinemaConsoleMock.Object,
            cinemaAccessorMock.Object,
            seatSelectionServiceMock.Object,
            screenServiceMock.Object,
            loggerMock.Object);
    }

    [TestMethod]
    [DataRow(MenuItemOption.BookTickets, true)]
    [DataRow(MenuItemOption.CheckBookings, false)]
    [DataRow(MenuItemOption.Exit, false)]
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