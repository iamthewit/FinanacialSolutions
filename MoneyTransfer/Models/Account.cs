using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.Models;

public class Account
{
    public Guid Id { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Account balance must be greater than zero.")]
    public decimal AccountBalance { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "Full name cannot be empty.")]
    public required string FullName { get; set; }
    
    [Required]
    [EnumDataType(typeof(AccountType), ErrorMessage = "Invalid account type.")]
    public AccountType AccountType { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "EmailAddress name cannot be empty.")]
    [EmailAddress]
    public required string EmailAddress { get; set; }
    
    [Required]
    [Phone]
    public required string PhoneNumber { get; set; }
}