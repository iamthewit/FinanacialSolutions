using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [MinLength(1)]
    public required string FirstName { get; set; }

    [Required]
    [MinLength(1)]
    public required string LastName { get; set; }

    [Required]
    [EmailAddress]
    public required string EmailAddress { get; set; }

    [Required]
    [MinLength(6)]
    public required string Password { get; set; }

    [Required]
    [Phone]
    public required string PhoneNumber { get; set; }
}
