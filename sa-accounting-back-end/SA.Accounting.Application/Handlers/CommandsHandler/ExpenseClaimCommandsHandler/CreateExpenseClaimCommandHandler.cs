using Mapster;
using Microsoft.AspNetCore.Identity;
using SA.Accounting.Application.Commands.ExpenseClaim;
using SA.Accounting.Application.Contracts.ExpenseClaims.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseClaimCommandsHandler;

public class CreateExpenseClaimCommandHandler(UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork,IExpenseClaimNumberGenerator expenseClaimNumberGenerator) : IRequestHandler<CreateExpenseClaimCommand, Result<CreateExpenseClaimResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IExpenseClaimNumberGenerator _expenseClaimNumberGenerator = expenseClaimNumberGenerator;
    public async Task<Result<CreateExpenseClaimResponse>> Handle(CreateExpenseClaimCommand command,CancellationToken cancellationToken)
    {
        var user = _userManager.Users
            .FirstOrDefault(x => x.Id == command.UserId);

        if (user is null)
            return Result.Failure<CreateExpenseClaimResponse>(UserErrors.NotFound);

        var claim = new ExpenseClaim
        {
            Number = await _expenseClaimNumberGenerator.GenerateAsync(cancellationToken),
            UserId = command.UserId,
            ClaimDate = DateOnly.FromDateTime(DateTime.Now)
        };

        await _unitOfWork.ExpenseClaims.AddAsync(claim, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success(claim.Adapt<CreateExpenseClaimResponse>());
    }
}
