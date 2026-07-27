using SA.Accounting.Application.Contracts.Custodies.Requests;
using SA.Accounting.Application.Contracts.Custodies.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Commands.Custody;

public record CreateCustodyMovementCommand(int CustodyId, CreateMovementRequest Request) : IRequest<Result<CustodyMovementResponse>>;
