namespace Domain.CinemaConsole;

public class CinemaConsole : ICinemaConsole
{
    public string? DefineCinema()
    {
        return Console.ReadLine();
    }

    public string? SelectMenuItem()
    {
        return Console.ReadLine();
    }

    public string? ReadNumberOfTicketsToBook()
    {
        return Console.ReadLine();
    }

    public string? ReadNewSeatPosition()
    {
        return Console.ReadLine();
    }
    
    public string? ReadBookingId()
    {
        return Console.ReadLine();
    }

    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }

    public void WriteEmptyLine()
    {
        Console.WriteLine();
    }
}