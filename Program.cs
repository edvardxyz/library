using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Text;



namespace biblo
{
    class Program
    {
        static void Main()
        {



        }
    }

    public class Interface
    {

        static void Run(){
        bool running = true;
            do
            {
                Library.getInput("Log ind\nSkriv bruger id: ");


            }while(running);
        }

        static string getPass()
        {
            string password = null;
            do
            {
                ConsoleKeyInfo keyinfo = Console.ReadKey(true);

                if(keyinfo.Key == ConsoleKey.Enter)
                    break;

                password += keyinfo.KeyChar;
            }while(true);
            return password;
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
        public string passHashed;
        public int id;
        public int booksBorrowed;

        public User(string name,string passHashed, int id, int booksBorrowed){

            this.name = name;
            this.passHashed = passHashed;
            this.id = id;
            this.booksBorrowed = booksBorrowed;
        }
    }

    public class Pass
    {
        public static string Hash(string passString)
        {
            SHA256 sha256obj = SHA256.Create();
            // SHA256 ComputeHash tager kun byte array, derfor konverteres string her;
            byte[] passBytes = sha256obj.ComputeHash(Encoding.UTF8.GetBytes(passString));

            // Encode tilbage til string fra 0 til length;
            return Encoding.UTF8.GetString(passBytes, 0, passBytes.Length);
        }
    }

    public class Library
    {
        private Dictionary<int, Book> books = new Dictionary<int, Book>();
        private Dictionary<int, User> users = new Dictionary<int, User>();
        private IFormatter formatter = new BinaryFormatter();

        public int AddUser(){
            int id = this.users.Count + 1;
            string name = getString("Indtast navn: ");
            string passHashed = Pass.Hash(Console.ReadLine());
            this.users.Add(id, new User(name, passHashed, id, 0));
            return id;
        }

        public void RemoveUser(int id)
        {
            this.users.Remove(id);
        }

        public void AddBook(int isbn){
            if(this.books.ContainsKey(isbn)){
                this.books[isbn].incCount();
            }else{
                string title = getString("Indtast bogens titel: ");
                string author = getString("Indtast bogens forfatter: ");
                string publisher = getString("Indtast bogens forlag: ");
                string genre = getString("Indtast bogens genre: ");
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
            Stream streambooks = new FileStream("books.dat", FileMode.Create, FileAccess.Write);
            formatter.Serialize(streambooks, books);
            streambooks.Close();

            Stream streamusers = new FileStream("users.dat", FileMode.Create, FileAccess.Write);
            formatter.Serialize(streamusers, books);
            streamusers.Close();

        }

        public void Import()
        {
            Stream streambooks = new FileStream("books.dat", FileMode.Open, FileAccess.Read);
            formatter.Serialize(streambooks, books);
            streambooks.Close();

            Stream streamusers = new FileStream("users.dat", FileMode.Open, FileAccess.Read);
            formatter.Serialize(streamusers, books);
            streamusers.Close();
        }

        public static string getString(string message)
        {
                Console.Clear();
                Console.Write(message);
                return Console.ReadLine();
        }

        public static int getInput(string message)
        {
            int number;
            do
            {
                Console.Clear();
                Console.Write(message);
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
