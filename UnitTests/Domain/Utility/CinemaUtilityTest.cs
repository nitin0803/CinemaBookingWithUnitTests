using Domain.Models;
using Domain.Utility;
using Shouldly;

namespace UnitTests.Domain.Utility;

[TestClass]
public class CinemaUtilityTest
{
    [TestMethod]
    [DataRow("", false)]
    public void AreCinemaDetailsValid_GivenCinemaDetails_ReturnsCorrectBooleanValue(
        string cinemaDetails,
        bool expectedResult)
    {
        // Arrange & Act
        var result = CinemaUtility.AreCinemaDetailsValid(cinemaDetails);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    [DataRow("", false)]
    public void IsNewSeatPositionValid_GivenRowLayouts_ReturnsCorrectBooleanValue(string newSeatPosition,
        bool expectedResult)
    {
        // Arrange
        var rowLayOuts = new List<RowLayOut>();

        // Act
        var result = CinemaUtility.IsNewSeatPositionValid(rowLayOuts, newSeatPosition);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    [DataRow("", false)]
    public void IsNumberOfTicketsValid_GivenNumberOfTicketsInput_ReturnsCorrectBooleanValue(
        string numberOfTicketsInput,
        bool expectedResult)
    {
        // Arrange & Act
        var result = CinemaUtility.IsNumberOfTicketsValid(numberOfTicketsInput);

        // Assert
        result.ShouldBe(expectedResult);
    }
}