using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MoneyTransfer.Models;

namespace MoneyTransfer.Dto;

public class AccountPatchDto
{
    public decimal? AccountBalance { get; set; }
    public string? FullName { get; set; }
    public AccountType? AccountType { get; set; }
    [EmailAddress]
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
}