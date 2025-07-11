using Microsoft.EntityFrameworkCore;
using MoneyTransfer.Models;

namespace MoneyTransfer;

public class FinancialSolutionsDbContext(DbContextOptions<FinancialSolutionsDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
}