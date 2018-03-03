using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Ideas.Models
{
    public class IdeasContext : DbContext
    {
        public IdeasContext()
            : base()
        { }

        public IdeasContext(DbContextOptions<IdeasContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<Idea>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Entity<Comment>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }

        public async Task<User> GetUserByCredentials(string Login, string Password)
        {
            return await this.User.FirstOrDefaultAsync(
                u => u.Login == Login.ToLowerInvariant() && u.Password == Password
            );
        }

        public string CreateTokenForUser(User user)
        {
            string value = Guid.NewGuid().ToString();
            Token token = new Token { User = user, Value = value };
            this.Add(token);
            this.SaveChanges();
            return value;
        }

        public async Task<User> GetUserFromCookies(IRequestCookieCollection cookies)
        {
            if (cookies.ContainsKey("Token"))
            {
                string value = cookies["Token"];
                Token token = await this.Token.Include(t => t.User).FirstOrDefaultAsync(t => t.Value == value);
                if (token != null)
                {
                    return token.User;
                }
            }
            return null;
        }

        public DbSet<Ideas.Models.User> User { get; set; }
        public DbSet<Ideas.Models.Idea> Idea { get; set; }
        public DbSet<Ideas.Models.Token> Token { get; set; }
        public DbSet<Ideas.Models.Comment> Comment { get; set; }
    }
}