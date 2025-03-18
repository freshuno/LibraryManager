using System;
using System.Diagnostics.Tracing;
using Microsoft.Win32.SafeHandles;
using System.Text.Json;

class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
    public Book(string title, string author, int year)
    {
        Title = title;
        Author = author;
        Year = year;
    }
}

class LibraryManager
{
    private List<Book> books = new List<Book>();
    public void AddBook(string title, string author, int year)
    {
        Book newBook = new Book(title, author, year);
        if (!books.Exists(b => b.Title == title))
        {
            books.Add(newBook);
        }
        else
        {
            System.Console.WriteLine("Such book already exists");
        }
    }
    public void RemoveBook(string title)
    {
        if (books.Exists(x => x.Title.ToLower() == title.ToLower()))
        {
            books.RemoveAll(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            System.Console.WriteLine("\nDeleted succesfully");
        }
        else
        {
            System.Console.WriteLine("\nThere is no such book in the library");
        }
    }
    public void ListBooks()
    {
        if (books.Count == 0)
        {
            System.Console.WriteLine("There are no books in the library");
            return;
        }
        foreach (var b in books)
        {
            System.Console.WriteLine($"{b.Title}, {b.Author}, {b.Year}");
        }
    }
    public void SaveToFile(string fileName)
    {
        if (books.Count() > 0)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    foreach (var b in books)
                    {
                        sw.WriteLine($"{b.Title}, {b.Author}, {b.Year}");
                    }
                    System.Console.WriteLine("Saved Succesfully to " + fileName);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Critical failure");
            }

        }
        else
        {
            System.Console.WriteLine("The library is empty!");
        }

    }
    public void ReadFromFile(string fileName, bool displayStatus)
    {
        string line = "";
        int countAdded = 0;
        int countExists = 0;
        try
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    if (parts.Count() < 3)
                    {
                        System.Console.WriteLine("Incorrenct number of lines, skipping");
                    }
                    int.TryParse(parts[2].Trim(), out int newYear);
                    if (!books.Exists(x => x.Title.Equals(parts[0].Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
                        AddBook(parts[0].Trim(), parts[1].Trim(), newYear);
                        countAdded++;
                    }
                    else
                    {
                        countExists++;
                    }
                }
                if (displayStatus)
                {
                    System.Console.WriteLine($"{countAdded} books added, {countExists} already exist");
                }
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
        }
    }
    public void SaveToJson(string fileName)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, jsonString);
            System.Console.WriteLine("Saved to " + fileName);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
        }
    }
    public void ReadFromJson(string fileName)
    {
        try
        {
            if (File.Exists(fileName))
            {
                string jsonString = File.ReadAllText(fileName);
                List<Book> importedBooks = JsonSerializer.Deserialize<List<Book>>(jsonString);

                int countAdded = 0;
                int countExists = 0;
                foreach (var book in importedBooks)
                {
                    if (!books.Exists(b => b.Title.Equals(book.Title, StringComparison.OrdinalIgnoreCase)))
                    {
                        books.Add(book);
                        countAdded++;
                    }
                    else
                    {
                        countExists++;
                    }
                }
                System.Console.WriteLine($"{countAdded} added, {countExists} already exist");
            }
            else
            {
                System.Console.WriteLine("JSON file doesn't exist.");
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine("Błąd podczas odczytu JSON: " + e.Message);
        }
    }
}

class Program
{
    public static void Main()
    {
        bool work = true;
        LibraryManager Library = new LibraryManager();
        Library.ReadFromFile("test.txt", false);
        while (work)
        {
            System.Console.WriteLine("\nChoose action:");
            System.Console.WriteLine("1. Add new book to library");
            System.Console.WriteLine("2. Remove book from library");
            System.Console.WriteLine("3. Display all books");
            System.Console.WriteLine("4. Export books to file");
            System.Console.WriteLine("5. Import books from file");
            System.Console.WriteLine("6. Save to JSON file");
            System.Console.WriteLine("7. Import from JSON file");
            System.Console.WriteLine("8. Quit");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        System.Console.WriteLine("\nThe title: ");
                        string title = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(title)){
                           System.Console.WriteLine("Title must not be empty!");
                           break;
                        }
                        System.Console.WriteLine("The author: ");
                        string author = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(author)){
                            System.Console.WriteLine("Author must not be empty");
                            break;
                        }
                        System.Console.WriteLine("Release year: ");
                        while (true)
                        {
                            if (int.TryParse(Console.ReadLine(), out int year))
                            {
                                Library.AddBook(title, author, year);
                                break;
                            }
                            System.Console.WriteLine("Type correct year.");
                        }
                        break;
                    case 2:
                        System.Console.WriteLine("\nType title: ");
                        string titleToRemove = Console.ReadLine();
                        Library.RemoveBook(titleToRemove);
                        break;
                    case 3:
                        Library.ListBooks();
                        break;
                    case 4:
                        Console.WriteLine("Type file name");
                        string fileName = Console.ReadLine();
                        Library.SaveToFile(fileName + ".txt");
                        break;
                    case 5:
                        Console.WriteLine("Type file name");
                        string fileNameSave = Console.ReadLine();
                        Library.ReadFromFile(fileNameSave + ".txt", true);
                        break;
                    case 6:
                        Console.WriteLine("Type JSON file name:");
                        string jsonFileName = Console.ReadLine();
                        Library.SaveToJson(jsonFileName + ".json");
                        break;
                    case 7:
                        Console.WriteLine("Type JSON file name:");
                        string jsonFileNameImport = Console.ReadLine();
                        Library.ReadFromJson(jsonFileNameImport + ".json");
                        break;
                    case 8:
                        System.Console.WriteLine("Quitting...");
                        work = false;
                        break;
                    default:
                        System.Console.WriteLine("Choose correct option");
                        break;
                }
            }
            else
            {
                System.Console.WriteLine("Choose correct option");
            }
        }
    }
}
