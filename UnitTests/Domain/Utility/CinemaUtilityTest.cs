using Domain.Models;
using Domain.Utility;
using Shouldly;

namespace UnitTests.Domain.Utility;

[TestClass]
public class CinemaUtilityTest
{
    [TestMethod]
    [DataRow("B04", 'B')]
    [DataRow("A04", 'A')]
    [DataRow("D99", 'D')]
    public void GetNewSeatPositionRowLabel_GivenSeatPosition_ReturnsCorrectResult(
        string seatPosition,
        char expectedResult)
    {
        // Arrange & Act
        var result = CinemaUtility.GetSeatPositionRowLabel(seatPosition);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    [DataRow("B04", 4)]
    [DataRow("A13", 13)]
    [DataRow("D99", 99)]
    public void GetSeatPositionNumber_GivenSeatPosition_ReturnsCorrectResult(string seatPosition, int expectedResult)
    {
        // Arrange & Act
        var result = CinemaUtility.GetSeatPositionNumber(seatPosition);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    public void GetNewBookingId_GivenCinemaBookings_ReturnsCorrectResult()
    {
        // Arrange
        var currentBookings = new List<Booking>
        {
            new("GIC1235", 5),
            new("GIC1236", 7),
        };
        
        // Act
        var result = CinemaUtility.GetNewBookingId(currentBookings);

        // Assert
        result.ShouldBe("GIC1237");
    }

    [TestMethod]
    [DataRow(7, 3, 4)]
    [DataRow(7, 4, 4)]
    [DataRow(9, 3, 5)]
    [DataRow(9, 4, 5)]
    [DataRow(10, 6, 5)]
    [DataRow(8, 8, 4)]
    [DataRow(12, 9, 7)]
    [DataRow(8, 7, 5)]
    public void GetMiddleSeatNumber_GivenSeatsCountInCurrentRowAndNumberOfTicketsToBook_ReturnsCorrectResult(
        int seatsCountInCurrentRow,
        int numberOfTicketsBook,
        int expectedResult)
    {
        // Arrange & Act
        var result = CinemaUtility.GetMiddleSeatNumber(seatsCountInCurrentRow, numberOfTicketsBook);
        
        // Assert
        result.ShouldBe(expectedResult);
    }
}