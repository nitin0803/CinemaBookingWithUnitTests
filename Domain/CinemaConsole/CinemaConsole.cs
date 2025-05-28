namespace Domain.CinemaConsole;

public class CinemaConsole : ICinemaConsole
{
    public string DefineCinema()
    {
        return Console.ReadLine();
    }

    public string SelectMenuItem()
    {
        return Console.ReadLine();
    }

    public string EnterNumberOfTicketsToBook()
    {
        return Console.ReadLine();
    }

    public string EnterNewSeatPosition()
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