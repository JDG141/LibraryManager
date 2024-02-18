using System;
using Microsoft.Data.Sqlite;
using BCrypt.Net;

public class DbManager
{
	// Private properties
	private string _connectionString = "Data Source=library.db";
	private SqliteConnection _connection;

	// Constructor
	public DbManager()
	{
		_connection = new SqliteConnection(_connectionString);
        _connection.Open();
		FirstTimeSetup();
	}

	public DbManager(string connectionString)
	{
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
		FirstTimeSetup();
    }

	// Destructor (automatically called when object is destroyed at end of program)
	~DbManager()
	{
        _connection.Close();
    }

	// Public methods
	public bool AuthUser(string username, string password)
	{
		// Check if user with username exists
		// Return true if they do, false if they don't

		string checkForUser = "SELECT COUNT(*) FROM users WHERE username = @username";
		SqliteCommand checkForUserCommand = new SqliteCommand(checkForUser, _connection);
		checkForUserCommand.Parameters.AddWithValue("@username", username);
		long userCount = (long)checkForUserCommand.ExecuteScalar();

		if (userCount == 0)
		{
            return false;
        }

		// Get hashed password
		string getHashedPassword = "SELECT password FROM users WHERE username = @username";
		SqliteCommand getHashedPasswordCommand = new SqliteCommand(getHashedPassword, _connection);
		getHashedPasswordCommand.Parameters.AddWithValue("@username", username);
		string hashedPassword = (string)getHashedPasswordCommand.ExecuteScalar();
		
		// Check if password is correct
		bool passwordCorrect = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

		if (!passwordCorrect)
		{
            return false;
        }

		// Get user's name
		string getName = "SELECT name FROM users WHERE username = @username";
		SqliteCommand getNameCommand = new SqliteCommand(getName, _connection);
		getNameCommand.Parameters.AddWithValue("@username", username);
		string name = (string)getNameCommand.ExecuteScalar();

		Console.WriteLine("Welcome back, " + name + "!\n");

		return true;
	}

	public int getUserId(string username)
	{
		string getUserId = "SELECT id FROM users WHERE username = @username";
		SqliteCommand getUserIdCommand = new SqliteCommand(getUserId, _connection);
		getUserIdCommand.Parameters.AddWithValue("@username", username);
		return Convert.ToInt32(getUserIdCommand.ExecuteScalar());
	}

	public void AddUser(string name, string username, string password)
	{
		string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
		string insertUser = "INSERT INTO users (name, username, password) VALUES (@name, @username, @password)";
		SqliteCommand insertUserCommand = new SqliteCommand(insertUser, _connection);
		insertUserCommand.Parameters.AddWithValue("@name", name);
		insertUserCommand.Parameters.AddWithValue("@username", username);
		insertUserCommand.Parameters.AddWithValue("@password", hashedPassword);
		insertUserCommand.ExecuteNonQuery();

		Console.WriteLine("User added successfully!");
	}

	public void DeleteUser(int id, int currentId)
	{
		// Check user to delete isn't the current user
		if (id == currentId)
		{
            Console.WriteLine("You can't delete yourself!");
            return;
        }

		// Delete user
		string deleteUser = "DELETE FROM users WHERE id = @id";
		SqliteCommand deleteUserCommand = new SqliteCommand(deleteUser, _connection);
		deleteUserCommand.Parameters.AddWithValue("@id", id);
		deleteUserCommand.ExecuteNonQuery();
		Console.WriteLine("User deleted successfully!");
	}

	public void AddBook(Book book)
	{
		string insertBook = "INSERT INTO books (title, author, year, pages, checkedOut) VALUES (@title, @author, @year, @pages, @checkedOut)";
		SqliteCommand insertBookCommand = new SqliteCommand(insertBook, _connection);
		insertBookCommand.Parameters.AddWithValue("@title", book.Title);
		insertBookCommand.Parameters.AddWithValue("@author", book.Author);
		insertBookCommand.Parameters.AddWithValue("@year", book.Year);
		insertBookCommand.Parameters.AddWithValue("@pages", book.Pages);
		insertBookCommand.Parameters.AddWithValue("@checkedOut", 0); // Not checked out, anything other than 0 is user id of borrower
		insertBookCommand.ExecuteNonQuery();
		Console.WriteLine("Book added successfully!");
	}

	public List<Book> GetAvailableBooks()
	{
		string getBooks = "SELECT * FROM books WHERE checkedOut = 0 ORDER BY title ASC";
		SqliteCommand getBooksCommand = new SqliteCommand(getBooks, _connection);
		SqliteDataReader books = getBooksCommand.ExecuteReader();
		
		List<Book> availableBooks = new List<Book>();
		while (books.Read())
		{
			int id = books.GetInt32(0);
            string title = books.GetString(1);
            string author = books.GetString(2);
            int year = books.GetInt32(3);
            int pages = books.GetInt32(4);
            int userId = books.GetInt32(5);

            availableBooks.Add(new Book(id, title, author, year, pages, userId));
		}

		return availableBooks;
	}

	public List<Book> GetCheckedoutBooks(int userId)
	{
        string getBooks = "SELECT * FROM books WHERE checkedOut = @userId ORDER BY title ASC";
        SqliteCommand getBooksCommand = new SqliteCommand(getBooks, _connection);
        getBooksCommand.Parameters.AddWithValue("@userId", userId);
        SqliteDataReader books = getBooksCommand.ExecuteReader();
        
        List<Book> checkedoutBooks = new List<Book>();
        while (books.Read())
		{
            checkedoutBooks.Add(new Book(
				books.GetInt32(0),
				books.GetString(1),
				books.GetString(2),
				books.GetInt32(3),
				books.GetInt32(4),
				books.GetInt32(5)
			));
        }

        return checkedoutBooks;
    }

	public void CheckoutBook(int bookId, int userId)
	{
        // Check if book is already checked out
		string checkBook = "SELECT COUNT(*) FROM books WHERE id = @bookId AND checkedOut != 0";
		SqliteCommand checkBookCommand = new SqliteCommand(checkBook, _connection);
		checkBookCommand.Parameters.AddWithValue("@bookId", bookId);
		long bookCount = (long)checkBookCommand.ExecuteScalar();

		if (bookCount > 0)
		{
            Console.WriteLine("Book is already checked out!");
            return;
        }

		// Checkout book
		string checkoutBook = "UPDATE books SET checkedOut = @userId WHERE id = @bookId";
		SqliteCommand checkoutBookCommand = new SqliteCommand(checkoutBook, _connection);
		checkoutBookCommand.Parameters.AddWithValue("@userId", userId);
		checkoutBookCommand.Parameters.AddWithValue("@bookId", bookId);
		checkoutBookCommand.ExecuteNonQuery();
		Console.WriteLine("Book checked out successfully!");
    }

	public void ReturnBook(int bookId, int userId)
	{
		// Check if book is checked out by user
		string checkBook = "SELECT COUNT(*) FROM books WHERE id = @bookId AND checkedOut = @userId";
		SqliteCommand checkBookCommand = new SqliteCommand(checkBook, _connection);
		checkBookCommand.Parameters.AddWithValue("@bookId", bookId);
		checkBookCommand.Parameters.AddWithValue("@userId", userId);
		long bookCount = (long)checkBookCommand.ExecuteScalar();
		
		if (bookCount == 0)
		{
            Console.WriteLine("You can't return a book you haven't checked out!");
            return;
        }

		// Return book
		string returnBook = "UPDATE books SET checkedOut = 0 WHERE id = @bookId";
		SqliteCommand returnBookCommand = new SqliteCommand(returnBook, _connection);
		returnBookCommand.Parameters.AddWithValue("@bookId", bookId);
		returnBookCommand.ExecuteNonQuery();
		Console.WriteLine("Book returned successfully!");
	}

	// Private methods
	void FirstTimeSetup()
	{
        // Create the books table
        string createTable = "CREATE TABLE IF NOT EXISTS books (id INTEGER PRIMARY KEY, title TEXT, author TEXT, year INTEGER, pages INTEGER, checkedOut INTEGER)";
        SqliteCommand createTableCommand = new SqliteCommand(createTable, _connection);
        createTableCommand.ExecuteNonQuery();

		// Create the users table
		createTable = "CREATE TABLE IF NOT EXISTS users (id INTEGER PRIMARY KEY, name TEXT, username TEXT UNIQUE, password BOOL)";
        createTableCommand = new SqliteCommand(createTable, _connection);
        createTableCommand.ExecuteNonQuery();

		// Create a user if one doesn't exist
		// Check if a user exists
		string checkForUser = "SELECT COUNT(*) FROM users";
		SqliteCommand checkForUserCommand = new SqliteCommand(checkForUser, _connection);
		long userCount = (long)checkForUserCommand.ExecuteScalar();

		if (userCount > 0)
		{
            return;
        }

		Console.WriteLine("Welcome to the library! Let's get you set up with an account.");
		string usersName = getStringFromUser("Enter your name: ");
		string usersUsername = getStringFromUser("Enter your username: ");
		string usersPassword = getStringFromUser("Enter your password: ");
		string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usersPassword);

		string insertUser = "INSERT INTO users (name, username, password) VALUES (@name, @username, @password)";
		SqliteCommand insertUserCommand = new SqliteCommand(insertUser, _connection);
		insertUserCommand.Parameters.AddWithValue("@name", usersName);
		insertUserCommand.Parameters.AddWithValue("@username", usersUsername);
		insertUserCommand.Parameters.AddWithValue("@password", hashedPassword);
		insertUserCommand.ExecuteNonQuery();

		Console.WriteLine("You're all set up! You can now check out books.\n");
    }

	static string getStringFromUser(string prompt)
	{
		Console.Write(prompt);
		return Console.ReadLine().Trim();
	}
}
