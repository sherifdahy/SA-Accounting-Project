using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Errors;

public static class FileErrors
{
    public static readonly Error NotFound = new(
        "File.NotFound",
        "File not found.",
        StatusCodes.Status404NotFound);
}
