using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Commands.Custody;
using SA.Accounting.Application.Contracts.Custodies.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Custodies;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.CustodyCommandsHandler;

public class CreateCustodyHandler(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager,ICustodyNumberGenerator custodyNumberGenerator) : IRequestHandler<CreateCustodyCommand, Result<CustodyDetailsResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ICustodyNumberGenerator _custodyNumberGenerator = custodyNumberGenerator;

    public async Task<Result<CustodyDetailsResponse>> Handle(CreateCustodyCommand command,CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken);

        if (user is null)
            return Result.Failure<CustodyDetailsResponse>(CustodyErrors.UserNotFound);

        var hasActive = _unitOfWork.Custodies.IsExist(x => x.UserId == command.UserId && !x.IsDisabled);

        if (hasActive)
            return Result.Failure<CustodyDetailsResponse>(CustodyErrors.UserHasActiveCustody);

        var custody = command.Request.Adapt<Custody>();

        custody.UserId = command.UserId;

        custody.Number = await _custodyNumberGenerator.GenerateAsync(cancellationToken);

        await _unitOfWork.Custodies.AddAsync(custody, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        var response = custody.Adapt<CustodyDetailsResponse>();

        return Result.Success(response);
    }
}
