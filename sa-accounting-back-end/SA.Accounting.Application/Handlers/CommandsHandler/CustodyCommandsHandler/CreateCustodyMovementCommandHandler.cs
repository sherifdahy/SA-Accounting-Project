using Mapster;
using SA.Accounting.Application.Commands.Custody;
using SA.Accounting.Application.Contracts.Custodies.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Custodies;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Enums;
using SA.Accounting.Infrastructure.Presistance.Data;
using SA.Accounting.Services.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Handlers.CommandsHandler.CustodyCommandsHandler;
public class CreateCustodyMovementHandler(IUnitOfWork unitOfWork,ICustodyBalanceCalculator balanceCalculator) : IRequestHandler<CreateCustodyMovementCommand, Result<CustodyMovementResponse>>
{
    private readonly ICustodyBalanceCalculator _balanceCalculator = balanceCalculator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CustodyMovementResponse>> Handle(CreateCustodyMovementCommand command,CancellationToken cancellationToken)
    {
        var request = command.Request;

        var custody = await _unitOfWork.Custodies
            .GetByIdAsync(command.CustodyId, cancellationToken);

        if (custody is null)
            return Result.Failure<CustodyMovementResponse>(CustodyErrors.NotFound);

        if (custody.IsDisabled)
            return Result.Failure<CustodyMovementResponse>(CustodyErrors.NotActive);

        if (IsOutflow(request.Type))
        {
            var currentBalance = await _balanceCalculator.GetBalanceAsync(
                custody.Id, cancellationToken);

            if (currentBalance < request.Amount)
                return Result.Failure<CustodyMovementResponse>(CustodyErrors.InsufficientBalance);
        }

        var movement = request.Adapt<CustodyMovement>();

        movement.CustodyId = custody.Id;

        await _unitOfWork.CustodyMovements.AddAsync(movement, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        var response = movement.Adapt<CustodyMovementResponse>();

        return Result.Success(response);
    }

    private static bool IsOutflow(MovementType type) =>
        type is MovementType.Return or MovementType.AdjustmentOut;
}
