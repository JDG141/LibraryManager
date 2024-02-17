using System;

public class Book
{
    // Public Properties
    public string Title { get; private set; }
    public string Author { get; private set; }
    public int Year { get; private set; }
    public int Pages { get; private set; }
    public int Id { get; private set; }
    public int UserId { get; private set; } // 0 if not checked out, otherwise user id of borrower

	// Constructor
	// Without id (for adding new books)
	public Book(string title, string author, int year, int pages)
	{
        Title = title;
        Author = author;
        Year = year;
        Pages = pages;
    }

	// With id (for getting books from the database)
	public Book(int id, string title, string author, int year, int pages, int userId)
	{
        Id = id;
        Title = title;
        Author = author;
        Year = year;
        Pages = pages;
        UserId = userId;
    }

	// Public methods
	public string getDetails()
	{
		return "Book " + Id + ": " + Title + " by " + Author + " (Released in " + Year + " with " + Pages + " page" + (Pages == 1 ? "" : "s") + ")";
	}
}