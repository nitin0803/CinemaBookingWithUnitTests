using Domain.Accessor;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace UnitTests.Domain.Accessor;

[TestClass]
public class CinemaAccessorTest
{
    private readonly Mock<ILogger<CinemaAccessor>> loggerMock =new();
    private readonly ICinemaAccessor _sut;

    public CinemaAccessorTest()
    {
        _sut = new CinemaAccessor(loggerMock.Object);
    }

    [TestMethod]
    public void CreateCinema_GivenMovieNameNumberOfRowsAndSeatsPerRow_CreateCinema()
    {
        // Arrange & Act
        var createdCinema = _sut.CreateCinema("TestMovieName", 2, 3);

        // Assert
        createdCinema.Movie.ShouldBe("TestMovieName");
        createdCinema.TotalRows.ShouldBe(2);
        createdCinema.SeatsPerRow.ShouldBe(3);
        createdCinema.Bookings.ShouldBeEmpty();
        createdCinema.TotalHallSeats.ShouldBe(6);
        createdCinema.TotalBookedSeats.ShouldBe(0);
        createdCinema.AvailableSeats.ShouldBe(6);

        var rowLayOuts = createdCinema.HallLayOut.RowLayOuts;
        rowLayOuts.Count.ShouldBe(2);

        var rowLayOut1 = rowLayOuts[0];
        rowLayOut1.RowLabel.ShouldBe('B');
        rowLayOut1.Seats.Count.ShouldBe(3);
        rowLayOut1.Seats[0].SeatNumber.ShouldBe(1);
        rowLayOut1.Seats[1].SeatNumber.ShouldBe(2);
        rowLayOut1.Seats[2].SeatNumber.ShouldBe(3);
        var rowLayOut2 = rowLayOuts[1];
        rowLayOut2.RowLabel.ShouldBe('A');
        rowLayOut2.Seats.Count.ShouldBe(3);
        rowLayOut2.Seats[0].SeatNumber.ShouldBe(1);
        rowLayOut2.Seats[1].SeatNumber.ShouldBe(2);
        rowLayOut2.Seats[2].SeatNumber.ShouldBe(3);
    }

    [TestMethod]
    public void GetCinema_ReturnsSameCreatedInstance()
    {
        // Arrange
        var createdCinema = _sut.CreateCinema("TestMovieName", 2, 3);

        // Act
        var result = _sut.GetCinema();

        // Assert
        result.ShouldBeSameAs(createdCinema);
    }

    [TestMethod]
    public void GetCinema_EveryTimeReturnsSameCreatedInstance()
    {
        // Arrange
        var createdCinema = _sut.CreateCinema("TestMovieName", 2, 3);

        // Act
        var result1 = _sut.GetCinema();
        var result2 = _sut.GetCinema();
        var result3 = _sut.GetCinema();

        // Assert
        result1.ShouldBeSameAs(createdCinema);
        result2.ShouldBeSameAs(createdCinema);
        result3.ShouldBeSameAs(createdCinema);
    }
    
    [TestMethod]
    public void AddBooking_AddNewBookingIntoCinema()
    {
        // Arrange
        var createdCinema = _sut.CreateCinema("TestMovieName", 2, 3);

        // Act
        _sut.AddBooking(new Booking("GIC1234", 4));

        // Assert
        createdCinema.Bookings.Count.ShouldBe(1);
        createdCinema.TotalHallSeats.ShouldBe(6);
        createdCinema.TotalBookedSeats.ShouldBe(4);
        createdCinema.AvailableSeats.ShouldBe(2);
    }
    
    [TestMethod]
    public void AddBooking_GivenNewBookingIdWithSameIdAsExitingBooking_ThrowsException()
    {
        // Arrange
        _sut.CreateCinema("TestMovieName", 2, 3);

        // Act
        _sut.AddBooking(new Booking("GIC1234", 4));

        // Act & Assert
        var exception = Should.Throw<Exception>(() => _sut.AddBooking(new Booking("GIC1234", 9)));
        exception.Message.ShouldBe("Exception occurred as booking already exist for booking id: GIC1234 !");
        
    }
    
    [TestMethod]
    public void TryGetBooking_GivenExistingBookingId_ReturnsSameBooking()
    {
        // Arrange
        _sut.CreateCinema("TestMovieName", 2, 3);
        _sut.AddBooking(new Booking("GIC1234", 4));

        // Act
        var result = _sut.TryGetBooking("GIC1234");

        // Assert
        result.ShouldNotBeNull();
        result.BookingId.ShouldBe("GIC1234");
        result.NumberOfBookedSeats.ShouldBe(4);
    }
    
    [TestMethod]
    public void TryGetBooking_GivenNonExistingBookingId_ReturnsNull()
    {
        // Arrange
        _sut.CreateCinema("TestMovieName", 2, 3);
        _sut.AddBooking(new Booking("GIC1234", 4));

        // Act
        var result = _sut.TryGetBooking("1245GIC1234");

        // Assert
        result.ShouldBeNull();
    }
}