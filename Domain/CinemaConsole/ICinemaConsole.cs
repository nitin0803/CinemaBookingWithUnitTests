namespace Domain.CinemaConsole;

public interface ICinemaConsole
{
    string DefineCinema();
    string SelectMenuItem();
    string EnterNumberOfTicketsToBook();
    string EnterNewSeatPosition();
    void WriteLine(string message);
    void WriteEmptyLine();
}