using Microsoft.IdentityModel.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BibliotekApi.Models
{
    public class Loan
    {
        public int Id { get; set; }

        [Required]
        public DateOnly LoanDate { get; set; }

        [Required]
        public DateOnly DueDate { get; set; }

        
        public DateOnly? ReturnDate { get; set; }

        public int BookId { get; set; }

        public Book Book { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
