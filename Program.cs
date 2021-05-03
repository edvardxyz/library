using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;



namespace biblo
{
    class Program
    {
        static void Main()
        {



        }
    }
    [Serializable]
    public class Book
    {
        public string title;
        public string author;
        public string publisher;
        public string genre;
        public int published;
        public int pages;
        public int isbn;
        private int countOfBook;

        public Book(string title, string author, string publisher, string genre, int published, int pages, int isbn){

            this.title = title;
            this.author = author;
            this.publisher = publisher;
            this.genre = genre;
            this.published = published;
            this.pages = pages;
            this.isbn = isbn;
            countOfBook = 1;
        }

        public void incCount(){
            countOfBook++;
        }

        public int decCount(){
            return --this.countOfBook;
        }

    }
    [Serializable]
    public class User
    {
        public string name;
        public int id;
        public int booksBorrowed;

        public User(string name, int id, int booksBorrowed){

            this.name = name;
            this.id = id;
            this.booksBorrowed = booksBorrowed;
        }
    }

    public class Library
    {
        private Dictionary<int, Book> books = new Dictionary<int, Book>();
        private Dictionary<int, User> users = new Dictionary<int, User>();
        private IFormatter formatter = new BinaryFormatter();

        public void AddBook(int isbn){
            if(this.books.ContainsKey(isbn)){
                this.books[isbn].incCount();
            }else{
                string title = Console.ReadLine();
                string author = Console.ReadLine();
                string publisher = Console.ReadLine();
                string genre = Console.ReadLine();
                int published = getInput("Skriv hvad år bogen blev udgivet: ");
                int pages = getInput("Skriv hvor mange sider bogen har: ");
                this.books.Add(isbn, new Book(title, author, publisher, genre, published, pages, isbn));
            }
        }

        public void RemoveBook(int isbn)
        {
            if(this.books.ContainsKey(isbn)){
                int thisBookCount = this.books[isbn].decCount();
                if(thisBookCount == 0)
                    this.books.Remove(isbn);
            }
        }

        public void ShowCollection()
        {
            if(this.books.Count > 0){
                foreach(Book book in this.books.Values){
                    Console.WriteLine(book.title);
                    Console.WriteLine(book.author);
                    Console.WriteLine(book.isbn);
                }
            }else{
                Console.WriteLine("Der er ikke nogen bøger i det her bibliotek :(");
            }
        }

        public void Export()
        {
            Stream stream = new FileStream("library.dat", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, books);
            stream.Close();
        }

        public void Import()
        {
            Stream stream = new FileStream("library.dat", FileMode.Open, FileAccess.Read);
            formatter.Serialize(stream, books);
            stream.Close();
        }

        static int getInput(string message)
        {
            int number;
            do
            {
                Console.Write(message);
            } while (!int.TryParse(Console.ReadLine(), out number));
            return number;
        }
    }
}

/*

Denne opgave skal i lave et nyt program til et bibliotek,

- Det første der skal laves er et klasse diagram over hvordan i vil opbygge programmet.

- Programmet skal dokumenters løbende og jeres kode skal indeholde kommentare med beskrivelse af hver klasse.

- Skriv programmet med den relevante funktionalitet (opret bog, opret forfatter, lån/aflever bog osv.)


Kode skal afleveres via github.

Dokumentation skal ligge i PDF format (PDF'en skal også indeholde klasse diagrammet med beskrivele).

*/
