using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Core.Abstractions;

public static class FileSettings
{
    public const int MaxFileSizeInMB = 30;
    public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
    public static readonly string[] AllowedSignatures = ["25-50"];
}
