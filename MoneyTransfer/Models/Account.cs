using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.Models;

public class Account
{
    public Guid Id { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Account balance must be greater than zero.")]
    public decimal AccountBalance { get; set; }
    
    [Required]
    [EnumDataType(typeof(AccountType), ErrorMessage = "Invalid account type.")]
    public AccountType AccountType { get; set; }
    
    // User reference (optional, if you want to link Account to User)
    public Guid UserId { get; set; }
    public User? User { get; set; }
}