using System;
using System.Security.Cryptography;
using System.Text;
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
            string PasswordMD5 = this.CalculateMD5Hash(Password);
            return await this.User.FirstOrDefaultAsync(
                u => u.Login == Login.ToLowerInvariant() && u.Password == PasswordMD5
            );
        }

        public async Task<bool> RegisterUser(User user)
        {
            user.Login = user.Login.ToLowerInvariant();

            User existing = await this.User.FirstOrDefaultAsync(
                u => u.Login == user.Login
            );

            if (existing == null)
            {
                user.Password = this.CalculateMD5Hash(user.Password);
                this.User.Add(user);
                await this.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public string CreateTokenForUser(User user)
        {
            string value = Guid.NewGuid().ToString();
            Token token = new Token { User = user, Value = value };
            this.Add(token);
            this.SaveChanges();
            return value;
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
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