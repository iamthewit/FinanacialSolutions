using System.ComponentModel.DataAnnotations;
using MoneyTransfer.Models;

namespace MoneyTransfer.Dto;

public class AccountPatchDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Account balance must be greater than zero.")]
    public decimal? AccountBalance { get; set; }
    
    public string? FullName { get; set; }
    
    [EnumDataType(typeof(AccountType), ErrorMessage = "Invalid account type.")]
    public AccountType? AccountType { get; set; }
    
    [EmailAddress]
    public string? EmailAddress { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
}