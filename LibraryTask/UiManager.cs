using System;

public class UiManager
{
    // Private properties
    private DbManager db;
    private int userId;

    // Constructor
    public UiManager(DbManager db)
    {
        this.db = db;
    }

    // Public methods
    public void Run()
    {
        // Login
        this.userId = login();

        bool readyToBreak = false;

        do
        {
            /*
             * Ask the user what they want to do
             * 1. Add user
             * 2. Delete user
             * 3. View available books
             * 4. View checked out books
             * 5. Add book
             * 6. Check out book
             * 7. Return book
             * 8. Exit
             */

            int userChoice = getIntFromUser("\nWhat would you like to do?\n1. Add user\n2. Delete user\n3. View available books\n4. View your checked out books\n5. Add book\n6. Check out book\n7. Return book\n8. Exit\n>>> ");

            switch (userChoice)
            {
                case 1:
                    addUser();
                    break;
                case 2:
                    deleteUser();
                    break;
                case 3:
                    viewBooks();
                    break;
                case 4:
                    viewCheckedoutBooks();
                    break;
                case 5:
                    addBook();
                    break;
                case 6:
                    checkoutBook();
                    break;
                case 7:
                    returnBook();
                    break;
                case 8:
                    readyToBreak = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        } while (!readyToBreak);

        Console.WriteLine("Goodbye!");
    }

    // Private methods
    static int getIntFromUser(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine().Trim();

        try
        {
            return int.Parse(input);
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            return getIntFromUser(prompt);
        }
    }

    static string getStringFromUser(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine().Trim();
    }

    int login()
    {
        Console.WriteLine("Welcome to the library!");

        bool readyToBreak = false;
        string username;

        do
        {
            username = getStringFromUser("Enter your username: ");
            string password = getStringFromUser("Enter your password: ");
            bool userExists = db.AuthUser(username, password);

            if (userExists)
            {
                readyToBreak = true;
            }
            else
            {
                Console.WriteLine("Incorrect login details, please try again.\n");
            }
        } while (!readyToBreak);

        return db.getUserId(username);
    }

    void addUser()
    {
        string name = getStringFromUser("Enter name: ");
        string username = getStringFromUser("Enter username: ");
        string password = getStringFromUser("Enter password: ");

        db.AddUser(name, username, password);
    }

    void deleteUser()
    {
        string username = getStringFromUser("Enter username: ");
        int userId = db.getUserId(username);
        db.DeleteUser(userId, this.userId);
    }

    void addBook()
    {
        Book myBook = new Book(
            getStringFromUser("Enter title: "),
            getStringFromUser("Enter author: "),
            getIntFromUser("Enter year: "),
            getIntFromUser("Enter pages: ")
        );

        db.AddBook(myBook);
    }

    void viewBooks()
    {
        List<Book> books = new List<Book>();
        books = db.GetAvailableBooks();

        // if no books are available, print a message
        if (books.Count == 0)
        {
            Console.WriteLine("No books available.");
            return;
        }

        Console.WriteLine("\nAvailable books:");
        foreach (Book book in books)
        {
            Console.WriteLine(book.getDetails());
        }
    }

    void viewCheckedoutBooks()
    {
        List<Book> books = new List<Book>();
        books = db.GetCheckedoutBooks(this.userId);

        // if no books are checked out, print a message
        if (books.Count == 0)
        {
            Console.WriteLine("No books checked out.");
            return;
        }

        Console.WriteLine("\nYour checked out books:");
        foreach (Book book in books)
        {
            Console.WriteLine(book.getDetails());
        }
    }

    void checkoutBook()
    {
        int bookId = getIntFromUser("Enter book id: ");
        db.CheckoutBook(bookId, this.userId);
    }

    void returnBook()
    {
        int bookId = getIntFromUser("Enter book id: ");
        db.ReturnBook(bookId, this.userId);
    }
}
