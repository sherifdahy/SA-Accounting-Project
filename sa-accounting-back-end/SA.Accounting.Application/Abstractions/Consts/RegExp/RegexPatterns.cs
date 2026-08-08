using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA.Accounting.Application.Abstractions.Consts.RegExp;
public static class RegexPatterns
{
    public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
    public const string PhoneNumber = "^01[0-9]{9}$";
    public const string SSN = @"^\d{14}$";
}
