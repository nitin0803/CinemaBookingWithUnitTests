using App.Controller;
using Domain.Accessor;
using Domain.CinemaConsole;
using Domain.Enums;
using Domain.Models;
using Moq;
using Service.MenuItemSelection;

namespace UnitTests.App;

[TestClass]
public class CinemaControllerTest
{
    private readonly Mock<ICinemaConsole> cinemaConsoleMock = new();
    private readonly Mock<ICinemaAccessor> cinemaAccessorMock = new();
    private readonly Mock<IMenuItemSelectionService> bookTicketsServiceMock = new();
    private readonly Mock<IMenuItemSelectionService> checkBookingsServiceMock = new();
    private readonly Mock<IMenuItemSelectionService> exitServiceMock = new();

    private readonly CinemaController sut;

    public CinemaControllerTest()
    {
        var menuItemSelectionServices = new List<IMenuItemSelectionService>()
        {
            bookTicketsServiceMock.Object,
            checkBookingsServiceMock.Object,
            exitServiceMock.Object
        };

        sut = new CinemaController(cinemaConsoleMock.Object, cinemaAccessorMock.Object, menuItemSelectionServices);
    }

    [TestMethod]
    public void StartCinemaApplication_CallsCreateCinemaAndHandleParticularMenuItemSelection()
    {
        // Arrange
        var cinema = Cinema.Create("TestMovieName", 2, 3);
        cinemaConsoleMock.Setup(m => m.DefineCinema())
            .Returns("TestMovieName 2 3");
        cinemaConsoleMock.Setup(m => m.SelectMenuItem())
            .Returns("Exit");
        cinemaAccessorMock.Setup(m => m.CreateCinema(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(cinema);
        exitServiceMock.Setup(m => m.IsResponsible(It.IsAny<MenuItemOption>()))
            .Returns(true);
        
        // Act
        sut.StartCinemaApplication();
        
        // Assert
        bookTicketsServiceMock.Verify(m => m.IsResponsible(It.IsAny<MenuItemOption>()), Times.Once());
        checkBookingsServiceMock.Verify(m => m.IsResponsible(It.IsAny<MenuItemOption>()), Times.Once());
        exitServiceMock.Verify(m => m.IsResponsible(It.IsAny<MenuItemOption>()), Times.Once());
        
        checkBookingsServiceMock.Verify(m => m.Handle(It.IsAny<MenuItemOption>()), Times.Never);
        bookTicketsServiceMock.Verify(m => m.Handle(It.IsAny<MenuItemOption>()), Times.Never);
        exitServiceMock.Verify(m => m.Handle(It.IsAny<MenuItemOption>()), Times.Once);
    }
}