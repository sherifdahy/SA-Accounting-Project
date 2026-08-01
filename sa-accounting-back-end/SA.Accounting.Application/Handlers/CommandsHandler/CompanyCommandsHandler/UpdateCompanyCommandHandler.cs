using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Companies;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.CompanyCommandsHandler;

public class UpdateCompanyCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCompanyCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _unitOfWork.Companies.FindAsync(
            x => x.Id == request.Id,
            cancellationToken,
            x => x.Owners,
            x => x.Accounts);
        if (company is null)
            return Result.Failure(CompanyErrors.NotFound);

        if (_unitOfWork.Companies.IsExist(x =>
            x.TaxRegistrationNumber == request.TaxRegistrationNumber && x.Id != request.Id))
            return Result.Failure(CompanyErrors.DuplicatedTaxRegistrationNumber);

        if (_unitOfWork.Companies.IsExist(x =>
            x.TaxFileNumber == request.TaxFileNumber && x.Id != request.Id))
            return Result.Failure(CompanyErrors.DuplicatedTaxFileNumber);

        // Update basic info
        company.Name = request.Name;
        company.TaxRegistrationNumber = request.TaxRegistrationNumber;
        company.TaxFileNumber = request.TaxFileNumber;
        company.Address = request.Address;

        #region Accounts Update

        var existingAccountIds = company.Accounts.Select(x => x.Id).ToHashSet();

        // Delete
        var accountsToDelete = company.Accounts
            .Where(a => !request.Accounts.Any(r => r.Id == a.Id));

        _unitOfWork.Accounts.DeleteRange(accountsToDelete);

        // New (ONLY Id == 0)
        var newAccounts = request.Accounts
            .Where(x => x.Id == 0)
            .Select(x =>
            {
                var acc = x.Adapt<Account>();
                acc.CompanyId = company.Id;
                return acc;
            }).ToList();

        await _unitOfWork.Accounts.AddRangeAsync(newAccounts, cancellationToken);

        // Update
        var updatedAccounts = request.Accounts
            .Where(x => x.Id != 0 && existingAccountIds.Contains(x.Id));

        foreach (var accountRequest in updatedAccounts)
        {
            var account = company.Accounts.First(a => a.Id == accountRequest.Id);

            accountRequest.Adapt(account);
            _unitOfWork.Accounts.Update(account);
        }

        #endregion

        #region Owners Update

        var existingOwnerIds = company.Owners.Select(x => x.Id).ToHashSet();

        // Delete
        var ownersToDelete = company.Owners
            .Where(o => !request.Owners.Any(r => r.Id == o.Id));

        _unitOfWork.Owners.DeleteRange(ownersToDelete);

        // New
        var newOwners = request.Owners
            .Where(x => x.Id == 0)
            .Select(x =>
            {
                var owner = x.Adapt<Owner>();
                owner.CompanyId = company.Id;
                return owner;
            }).ToList();

        await _unitOfWork.Owners.AddRangeAsync(newOwners, cancellationToken);

        // Update
        var updatedOwners = request.Owners
            .Where(x => x.Id != 0 && existingOwnerIds.Contains(x.Id));

        foreach (var ownerRequest in updatedOwners)
        {
            var owner = company.Owners.First(x => x.Id == ownerRequest.Id);

            ownerRequest.Adapt(owner);
            _unitOfWork.Owners.Update(owner);
        }

        #endregion

        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
