using ContractMontlyClaimSystemPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ContractMontlyClaimSystemPOE.Data
{
    public class ApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
    }
}
