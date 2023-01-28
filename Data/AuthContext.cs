using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Data;

public class AuthContext : IdentityDbContext<IdentityUser>
{
    public AuthContext(DbContextOptions<AuthContext> options) : base(options)
    {
    }
}