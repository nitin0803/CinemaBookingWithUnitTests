namespace Domain.CinemaConsole;

public interface ICinemaConsole
{
    string? DefineCinema();
    string? SelectMenuItem();
    string? ReadNumberOfTicketsToBook();
    string? ReadNewSeatPosition();
    string? ReadBookingId();
    void WriteLine(string message);
    void WriteEmptyLine();
}