using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.Files.Responses;

public record FileResponse(
    Guid Id,
    string FileName
);
