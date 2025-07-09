using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.Models;

public class Account
{
    public int Id { get; set; }
    public decimal AccountBalance { get; set; }
    [Required]
    [MinLength(1, ErrorMessage = "Full name cannot be empty.")]
    public required string FullName { get; set; }
    public AccountType AccountType { get; set; }
    [Required]
    [MinLength(1, ErrorMessage = "EmailAddress name cannot be empty.")]
    [EmailAddress]
    public required string EmailAddress { get; set; }
    public required string PhoneNumber { get; set; }
}