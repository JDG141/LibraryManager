using System;

public class Book
{
	// Private Properties
	private string _title;
	private string _author;
	private int _id;
	private int _year;
	private int _pages;
	private int _userId; // 0 if not checked out, otherwise user id of borrower

	// Constructor
	// Without id (for adding new books)
	public Book(string title, string author, int year, int pages)
	{
        _title = title;
        _author = author;
        _year = year;
        _pages = pages;
    }

	// With id (for getting books from the database)
	public Book(int id, string title, string author, int year, int pages, int userId)
	{
        _id = id;
        _title = title;
        _author = author;
        _year = year;
        _pages = pages;
        _userId = userId;
    }

	// Public methods
	public string getTitle()
	{
        return _title;
    }

	public string getAuthor()
	{
		   return _author;
	}

	public int getYear()
	{
        return _year;
    }

	public int getPages()
	{
		return _pages;
	}

	public int getId()
	{
        return _id;
    }

	public int getBorrowerId()
	{
        return _userId;
    }

	public string getDetails()
	{
		return "Book " + _id + ": " + _title + " by " + _author + " (Released in " + _year + " with " + _pages + " page" + (_pages == 1 ? "" : "s") + ")";
	}
}