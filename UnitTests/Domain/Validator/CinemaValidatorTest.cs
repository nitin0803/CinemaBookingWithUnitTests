using Domain.Models;
using Domain.Validator;
using Shouldly;

namespace UnitTests.Domain.Validator;

[TestClass]
public class CinemaValidatorTest
{
    [TestMethod]
    [DataRow("", false)]
    [DataRow(null, false)]
    [DataRow("someContinuousString", false)]
    [DataRow("some, Continuous, String", false)]
    [DataRow("some Continuous String", false)]
    [DataRow("some 12fgd 45ng", false)]
    [DataRow("TestMovie 0 49", false)]
    [DataRow("TestMovie 25 0", false)]
    [DataRow("TestMovie 27 12", false)]
    [DataRow("TestMovie 26 51", false)]
    [DataRow("TestMovie 26 50", true)]
    [DataRow("TestMovie 06 49", true)]
    public void AreCinemaDetailsValid_GivenCinemaDetails_ReturnsCorrectResult(
        string cinemaDetails,
        bool expectedResult)
    {
        // Arrange & Act
        var result = CinemaValidator.AreCinemaDetailsValid(cinemaDetails);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    [DataRow("ABCD01", false)]
    [DataRow("A", false)]
    [DataRow("A01B", false)]
    [DataRow("01B", false)]
    [DataRow("A03", false)]
    [DataRow("B07", false)]
    [DataRow("C01", false)]
    [DataRow("A01", true)]
    [DataRow("B02", true)]
    public void IsNewSeatPositionValid_GivenRowLayouts_ReturnsCorrectResult(
        string newSeatPosition,
        bool expectedResult)
    {
        // Arrange
        var rowLayOuts = new List<RowLayOut>()
        {
            new('A', new[] { new Seat(1), new Seat(2) }),
            new('B', new[] { new Seat(1), new Seat(2) })
        };

        // Act
        var result = CinemaValidator.IsNewSeatPositionValid(rowLayOuts, newSeatPosition);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    [DataRow("", false)]
    [DataRow(null, false)]
    [DataRow("0", false)]
    [DataRow("1", true)]
    [DataRow("12", true)]
    public void IsNumberOfTicketsToBookValid_GivenNumberOfTicketsToBookInput_ReturnsCorrectResult(
        string numberOfTicketsInput,
        bool expectedResult)
    {
        // Arrange & Act
        var result = CinemaValidator.IsNumberOfTicketsToBookValid(numberOfTicketsInput);

        // Assert
        result.ShouldBe(expectedResult);
    }
}