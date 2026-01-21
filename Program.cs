
using BibliotekApi.Data;
using BibliotekApi.Models;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace BibliotekApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {

                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/books", async (LibraryDbContext context) =>
            {
                var books = await context.Books.ToListAsync();

                return Results.Ok(books);
            });

            app.MapGet("/books/{id}", async (LibraryDbContext context, int id) =>
            {
                var book = await context.Books.FirstOrDefaultAsync(b => b.Id == id);

                return Results.Ok(book);
            });

            app.MapPost("/books", async (LibraryDbContext context, Book book) =>
            {
                context.Books.Add(book);
                await context.SaveChangesAsync();

                return Results.Created($"/books/{book.Id}", book);
            });

            app.MapPut("/books{id}", async (LibraryDbContext context, int id, Book updatedBook) =>
            {
                var book = await context.Books.FirstOrDefaultAsync(b => b.Id == id);

                book.Title = updatedBook.Title;
                book.Author = updatedBook.Author;
                book.ISBN = updatedBook.ISBN;

                await context.SaveChangesAsync();

                return Results.Ok(book);
            });

            app.MapDelete("/books/{id}", async (LibraryDbContext context, int id) =>
            {
                var book = await context.Books.FirstOrDefaultAsync(b => b.Id == id);

                context.Books.Remove(book);

                await context.SaveChangesAsync();

                return Results.NoContent();
            });

            app.MapGet("/users", async (LibraryDbContext context) =>
            {
                var users = await context.Users.ToListAsync();

                return Results.Ok(users);
            });

            app.MapGet("/users/{id}", async (LibraryDbContext context, int id) =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

                return Results.Ok(user);
            });

            app.MapPost("/users", async (LibraryDbContext context, User user) =>
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();

                return Results.Created($"/users/{user.Id}", user);
            });

            app.MapPut("/users{id}", async (LibraryDbContext context, int id, User updatedUser) =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                user.PhoneNr = updatedUser.PhoneNr;

                await context.SaveChangesAsync();

                return Results.Ok(user);
            });

            app.MapPost("/loans", async (LibraryDbContext context, Loan loan) =>
            {               
                var book = await context.Books.FirstOrDefaultAsync(b => b.Id == loan.BookId);
                if (book == null || !book.Status)
                {
                    return Results.NotFound();
                }

                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == loan.UserId);
                if (user == null)
                {
                    return Results.NotFound();
                }

                loan.LoanDate = DateOnly.FromDateTime(DateTime.Now);
                                
                book.Status = false;

                context.Loans.Add(loan);
                await context.SaveChangesAsync();

                return Results.Created($"/loans/{loan.Id}", new
                {
                    loan.Id,
                    loan.BookId,
                    loan.UserId,
                    loan.LoanDate,
                    loan.ReturnDate
                });
            });

            app.MapGet("/loans/active", async (LibraryDbContext context) =>
            {
                var loans = await context.Loans.Where(l => l.ReturnDate == null).ToListAsync();

                return loans;
            });

            app.MapPut("/loans/{id}/return", async (LibraryDbContext context, int id) =>
            {
                var loan = await context.Loans.FirstOrDefaultAsync(l => l.Id == id);

                loan.ReturnDate = DateOnly.FromDateTime(DateTime.Now);

                var book = await context.Books.FirstOrDefaultAsync(b => b.Id == loan.BookId);

                book.Status = true;

                await context.SaveChangesAsync();

                return Results.Ok();
                
            });


            app.Run();
        }
    }
}
