using SA.Accounting.Application.Contracts.Custodies.Requests;
using SA.Accounting.Application.Contracts.Custodies.Responses;

namespace SA.Accounting.Application.Commands.Custody;

public record CreateCustodyCommand(int UserId,CreateCustodyRequest Request) : IRequest<Result<CustodyDetailsResponse>>;
