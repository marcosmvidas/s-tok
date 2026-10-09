using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace s_tok.Models
{
    public class S_tokDbContext : IdentityDbContext
    {
        public S_tokDbContext(DbContextOptions<S_tokDbContext> options)
            : base(options)
        {
        }
    }
}
