using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Queries.File;

public record GetFileQuery(Guid Id) : IRequest<Result<(byte[] fileContent, string contentType, string fileName)>>;
