namespace Domain.Models;

public class HallLayout(IReadOnlyList<RowLayOut> rowLayOuts)
{
    public IReadOnlyList<RowLayOut> RowLayOuts { get; } = rowLayOuts;
}