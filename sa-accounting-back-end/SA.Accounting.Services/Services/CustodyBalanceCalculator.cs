using SA.Accounting.Core.Entities.Custodies;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Enums;

namespace SA.Accounting.Services.Services;

public class CustodyBalanceCalculator(IUnitOfWork unitOfWork) : ICustodyBalanceCalculator
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CustodyBalance> CalculateAsync(int custodyId,CancellationToken cancellationToken = default)
    {
        var sums = (await _unitOfWork.CustodyMovements
            .FindAllAsync(x => x.CustodyId == custodyId, [],cancellationToken))
            .GroupBy(x => x.Type)
            .Select(g => new
            {
                Type = g.Key,
                Total = g.Sum(x => x.Amount)
            })
            .ToList();

        decimal Get(MovementType type) =>
            sums.FirstOrDefault(s => s.Type == type)?.Total ?? 0m;

        var deposits = Get(MovementType.Deposit);
        var approvedExpenses = Get(MovementType.ApprovedExpense);
        var returns = Get(MovementType.Return);
        var adjustmentsIn = Get(MovementType.AdjustmentIn);
        var adjustmentsOut = Get(MovementType.AdjustmentOut);

        var balance = deposits + adjustmentsIn - approvedExpenses - returns - adjustmentsOut;

        return new CustodyBalance(
            balance,
            deposits,
            approvedExpenses,
            returns,
            adjustmentsIn,
            adjustmentsOut);
    }
    public async Task<decimal> GetBalanceAsync(int custodyId,CancellationToken cancellationToken = default)
    {
        var result = (await _unitOfWork.CustodyMovements
            .FindAllAsync(x => x.CustodyId == custodyId, [],cancellationToken))
            .GroupBy(x => 1)
            .Select(g => new
            {
                Inflow = g.Where(x => x.Type == MovementType.Deposit
                                   || x.Type == MovementType.AdjustmentIn)
                          .Sum(x => (decimal?)x.Amount) ?? 0m,
                Outflow = g.Where(x => x.Type == MovementType.ApprovedExpense
                                    || x.Type == MovementType.Return
                                    || x.Type == MovementType.AdjustmentOut)
                           .Sum(x => (decimal?)x.Amount) ?? 0m
            })
            .FirstOrDefault();

        return (result?.Inflow ?? 0m) - (result?.Outflow ?? 0m);
    }
}
