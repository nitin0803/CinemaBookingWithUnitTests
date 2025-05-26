using Domain.Models;

namespace UnitTests.Service;

[TestClass]
public class CinemaAccessorTest
{
    [TestMethod]
    [DataRow("", false)]
    public void CreateCinemaTest_GivenRequiredDetails_CinemaIsCreated(string movie, int rows, int seatsPerRow)
    {
        
    }

    [TestMethod]
    [DataRow("", false)]
    public void GetCinema_GivenMultipleInvoke_ReturnsSameSingleCinema()
    {
        
    }

    [TestMethod]
    [DataRow("", false)]
    public void AddBooking(Booking booking)
    {
        
    }

    [TestMethod]
    [DataRow("", false)]
    public void TryGetBooking(string bookingId)
    {
        
    }
}