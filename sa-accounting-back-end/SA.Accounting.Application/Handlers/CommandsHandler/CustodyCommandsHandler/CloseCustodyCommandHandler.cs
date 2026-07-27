using SA.Accounting.Application.Commands.Custody;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Infrastructure.Presistance.Data;
using SA.Accounting.Services.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Handlers.CommandsHandler.CustodyCommandsHandler;

public class CloseCustodyHandler(IUnitOfWork unitOfWork,ICustodyBalanceCalculator custodyBalanceCalculator) : IRequestHandler<CloseCustodyCommand, Result>
{
    private readonly ICustodyBalanceCalculator _balanceCalculator = custodyBalanceCalculator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(CloseCustodyCommand command,CancellationToken cancellationToken)
    {
        var custody = await _unitOfWork.Custodies.GetByIdAsync(command.Id,cancellationToken);

        if (custody is null)
            return Result.Failure(CustodyErrors.NotFound);

        if (custody.IsDisabled)
            return Result.Failure(CustodyErrors.NotActive);

        var balance = await _balanceCalculator.GetBalanceAsync(custody.Id, cancellationToken);

        if (balance != 0m)
            return Result.Failure(CustodyErrors.CannotCloseWithBalance);

        custody.IsDisabled = true;
        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
