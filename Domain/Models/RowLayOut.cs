namespace Domain.Models;

public record RowLayOut(char RowLabel, IReadOnlyList<Seat> Seats);