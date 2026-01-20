using BibliotekApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotekApi.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options): base(options)
        {
            
        }

        

        public DbSet<Book> Books { get; set; }       
            
        public DbSet<Loan> Loans { get; set; }

        public DbSet<User> Users { get; set; }

    }
}
