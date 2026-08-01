using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Contracts.Company.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Companies;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.CompanyCommandsHandler;

public class CreateCompanyCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCompanyCommand, Result<CompanyDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CompanyDetailResponse>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        if (_unitOfWork.Companies.IsExist(x => x.TaxRegistrationNumber == request.TaxRegistrationNumber))
            return Result.Failure<CompanyDetailResponse>(CompanyErrors.DuplicatedTaxRegistrationNumber);

        if (_unitOfWork.Companies.IsExist(x => x.TaxFileNumber == request.TaxFileNumber))
            return Result.Failure<CompanyDetailResponse>(CompanyErrors.DuplicatedTaxFileNumber);

        var oData = request.Adapt<Company>();

        await _unitOfWork.Companies.AddAsync(oData, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        var company = await _unitOfWork.Companies.FindAsync(
            x => x.Id == oData.Id,
            ["Accounts.Platform", "Owners"],
            cancellationToken);
        
        return Result.Success(company.Adapt<CompanyDetailResponse>());
    }
}
