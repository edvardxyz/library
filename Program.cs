using System;
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
            Interface.Run();
        }
    }

    public class Interface
    {

        public static void Run(){
            Library lib = new Library();
            lib.Import();
            int loginID;
            bool login = false;
            int passTries;

RunMenu:
            int menu = GetDigit("1) Bruger login\n2) Opret bruger\n3) Admin login\n4) Exit", 4);

            switch (menu)
            {
                // bruger login
                case 1:
                    passTries = 5;
                    loginID = GetInput("Log ind\nSkriv bruger id: ");
                    if(lib.UserExists(loginID)){
                        do{
                            if(lib.CheckPass(loginID, GetPass())){
                                login = true;
                                break;
                            }else{
                                Console.WriteLine("Forkert kode!\n{0} forsøg tilbage\nTryk enter for at prøve igen.\nEller Escape for at lukke",passTries);
                                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                    break;
                            }
                        }while(--passTries > 0);

                    }else{
                        Console.WriteLine("Bruger eksistere ikke\nTryk en tast for at fortsætte");
                        Console.ReadKey(true);
                    }
                    if(login)
                        UserMenu(lib, loginID);
                    login = false;
                    goto RunMenu;

                    // opret ny bruger
                case 2:
                    Console.Clear();
                    Console.WriteLine("Dit bruger id er: {0}", lib.AddUser());
                    Console.WriteLine("Brug dit id til at logge ind");
                    Console.WriteLine("\nTryk for at gå tilbage");
                    Console.ReadKey(true);
                    goto RunMenu;

                    // admin login
                case 3:
                    passTries = 5;
                    do{
                            if(GetPass() == "aac2cb9a3eceed43645c27cb355611b0c48afae5320d53f6ffe6f7e8a28cd589"){
                                login = true;
                                break;
                            }else{
                                Console.WriteLine("Forkert kode!\n{0} forsøg tilbage\nTryk enter for at prøve igen.\nEller Escape for at lukke",passTries);
                                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                    break;
                            }
                        }while(--passTries > 0);
                    if(login)
                        AdminMenu(lib);
                    login = false;
                    goto RunMenu;
                    // Exit
                case 4:
                    break;
                default:
                    Console.WriteLine("Ikke en menu!");
                    Console.ReadKey(true);
                    goto RunMenu;
            }

            lib.Export();
        }

        private static void UserMenu(Library lib, int id){
UserMenu:
            int menu = GetDigit("1) Lån bog\n2) Aflever bog\n3) Se hvilke bøger du har lånt\n4) Se hvilke bøger biblioteket har\n5) Slet bruger\n6) Log ud", 6);

            switch (menu)
            {
                case 1:
                    lib.BorrowBook(id, GetInput("Indtast ISBN på bogen du vil låne: "));
                    goto UserMenu;
                case 2:
                    lib.ReturnBook(id, GetInput("Indtast ISBN på bogen du vil aflevere: "));
                    goto UserMenu;
                case 3:
                    lib.ShowUserBorrowed(id);
                    goto UserMenu;
                case 4:
                    lib.ShowCollection();
                    goto UserMenu;
                case 5:
                    do{
                        Console.WriteLine("\nEr du sikker på du vil slette din bruger?");
                        Console.WriteLine("Tryk 'y' for at bekræfte, tryk en anden tast for at fortryde");
                        if (Console.ReadKey(true).Key == ConsoleKey.Y){
                            lib.RemoveUser(id);
                            goto case 6;
                        }else{
                            break;
                        }
                    }while(true);
                    goto UserMenu;
                case 6:
                    break;
                default:
                    goto UserMenu;
            }
        }

        private static void AdminMenu(Library lib){
UserMenu:
            int menu = GetDigit("1) Opret bog\n2) Slet bog\n3) Slet bruger\n4) Vis alle bøger\n5) Vis alle brugere\n6) Log ud", 6);

            switch (menu)
            {
                case 1:
                    lib.AddBook(GetInput("Indtast bogens ISBN nummer: "));
                    goto UserMenu;
                case 2:
                    lib.RemoveBook(GetInput("Indtast bogens ISBN nummber: "));
                    goto UserMenu;
                case 3:
                    lib.RemoveUser(GetInput("Indtast brugerens ID: "));
                    goto UserMenu;
                case 4:
                    lib.ShowCollection();
                    goto UserMenu;
                case 5:
                    lib.ShowUsers();
                    goto UserMenu;
                case 6:
                    break;
                default:
                    goto UserMenu;
            }
        }

        public static string GetString(string message)
        {
            Console.Clear();
                Console.Write(message);
                return Console.ReadLine();
        }

        public static int GetInput(string message)
        {
            int number;
            do
            {
                Console.Clear();
                Console.Write(message);
            } while (!int.TryParse(Console.ReadLine(), out number));
            return number;
        }

        public static int GetDigit(string message, int max)
        {
            int number;
            do
            {
                Console.Clear();
                Console.Write(message);
                number = Console.ReadKey(true).KeyChar - '0';
            } while (number > max || number < 1);
            return number;
        }

        public static string GetPass()
        {
            Console.Clear();
            Console.Write("Indtast kode: ");
            string password = null;
            do
            {
                ConsoleKeyInfo keyinfo = Console.ReadKey(true);

                if(keyinfo.Key == ConsoleKey.Enter)
                    break;

                password += keyinfo.KeyChar;
            }while(true);
            return Pass.Hash(password);
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
        public int countOfBook;

        public Book(string title, string author, string publisher, string genre, int published, int pages, int isbn){

            this.title = title;
            this.author = author;
            this.publisher = publisher;
            this.genre = genre;
            this.published = published;
            this.pages = pages;
            this.isbn = isbn;
            this.countOfBook = 1;
        }

        public void incCount(){
            this.countOfBook++;
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
        public int borrowedCount;
        private Dictionary<int, Book> booksBorrowed = new Dictionary<int, Book>();

        public User(string name,string passHashed, int id, int borrowedCount){
            this.name = name;
            this.passHashed = passHashed;
            this.id = id;
            this.borrowedCount = borrowedCount;
        }
        public bool HasBook(int isbn){
            if(this.booksBorrowed.ContainsKey(isbn))
                return true;
            else
                return false;
        }

        public void Borrow(Book book){
            if(booksBorrowed.ContainsKey(book.isbn)){
                booksBorrowed[book.isbn].incCount();
                this.borrowedCount++;
            }
            else{
                Book copyBook = new Book(
                        book.title,
                        book.author,
                        book.publisher,
                        book.genre,
                        book.published,
                        book.pages,
                        book.isbn);
                booksBorrowed.Add(book.isbn, copyBook);
                this.borrowedCount++;
            }
        }

        public Book Return(int isbn){
            Book returnBook = new Book(
                    booksBorrowed[isbn].title,
                    booksBorrowed[isbn].author,
                    booksBorrowed[isbn].publisher,
                    booksBorrowed[isbn].genre,
                    booksBorrowed[isbn].published,
                    booksBorrowed[isbn].pages,
                    booksBorrowed[isbn].isbn);

            if(booksBorrowed[isbn].decCount() < 1)
                booksBorrowed.Remove(isbn);
            return returnBook;
        }

        public void ShowBorrowed()
        {
            Console.Clear();
            Console.WriteLine("\n=======================================\n=======================================");
            if(this.booksBorrowed.Count > 0){
                foreach(Book book in this.booksBorrowed.Values){
                    Console.WriteLine("Titel: {0}", book.title);
                    Console.WriteLine("Forfatter: {0}",book.author);
                    Console.WriteLine("ISBN: {0}",book.isbn);
                    Console.WriteLine("Genre:: {0}",book.genre);
                    Console.WriteLine("Sider: {0}",book.pages);
                    Console.WriteLine("Udgivet: {0}",book.published);
                    Console.WriteLine("Forlag: {0}",book.publisher);
                    Console.WriteLine("Antal: {0}",book.countOfBook);
                    Console.WriteLine("=======================================");
                }
            }else{
                Console.WriteLine("Du har ikke lånt nogen bøger :(");
            }
            Console.WriteLine("=======================================");
            Console.WriteLine("Tryk en tast for at gå tilbage");
            Console.ReadKey(true);
        }
    }

    public class Pass
    {
        public static string Hash(string passString)
        {
            string passHashString = "";
            SHA256 sha256obj = SHA256.Create();
            // SHA256 ComputeHash tager kun byte array, derfor konverteres string her;
            byte[] passBytes = sha256obj.ComputeHash(Encoding.UTF8.GetBytes(passString));

            for ( int i =0; i < passBytes.Length; i++){

                passHashString += passBytes[i].ToString("x2");
            }
            return passHashString;
        }
    }

    public class Library
    {
        private Dictionary<int, Book> books = new Dictionary<int, Book>();
        private Dictionary<int, User> users = new Dictionary<int, User>();
        private IFormatter formatter = new BinaryFormatter();

        public bool CheckPass(int id, string pass){
            if(this.users[id].passHashed == pass)
                return true;
            else
                return false;
        }

        public void ReturnBook(int id, int isbn){
            if(users[id].HasBook(isbn)){
                if(this.books.ContainsKey(isbn)){
                    this.books[isbn].incCount();
                }else{
                    this.books.Add(isbn, this.users[id].Return(isbn));
                }
            }
        }

        public void ShowUserBorrowed(int id){
            users[id].ShowBorrowed();
        }

        public bool BookExists(int isbn){
            if(this.books.ContainsKey(isbn))
                return true;
            else
                return false;
        }

        public bool BorrowBook(int id, int isbn){
            if(BookExists(isbn)){
                this.users[id].Borrow(books[isbn]);
                RemoveBook(isbn);
                return true;
            }
            else
                return false;
        }

        public bool UserExists(int id){
            if(this.users.ContainsKey(id))
                return true;
            else
                return false;
        }

        public int getHighestID(){
            int highID = 0;
                foreach(User user in this.users.Values){
                    if(highID < user.id)
                        highID = user.id;
                }
            return highID;
        }

        public int AddUser(){
            int id = getHighestID() + 1;
            string name = Interface.GetString("Indtast navn: ");
            string passHashed = Interface.GetPass();
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
                string title = Interface.GetString("Indtast bogens titel: ");
                string author = Interface.GetString("Indtast bogens forfatter: ");
                string publisher = Interface.GetString("Indtast bogens forlag: ");
                string genre = Interface.GetString("Indtast bogens genre: ");
                int published = Interface.GetInput("Skriv hvad år bogen blev udgivet: ");
                int pages = Interface.GetInput("Skriv hvor mange sider bogen har: ");
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
            Console.Clear();
            Console.WriteLine("\n=======================================\n=======================================");
            if(this.books.Count > 0){
                foreach(Book book in this.books.Values){
                    Console.WriteLine("Titel: {0}", book.title);
                    Console.WriteLine("Forfatter: {0}",book.author);
                    Console.WriteLine("ISBN: {0}",book.isbn);
                    Console.WriteLine("Genre:: {0}",book.genre);
                    Console.WriteLine("Sider: {0}",book.pages);
                    Console.WriteLine("Udgivet: {0}",book.published);
                    Console.WriteLine("Forlag: {0}",book.publisher);
                    Console.WriteLine("Antal: {0}",book.countOfBook);
                    Console.WriteLine("=======================================");
                }
            }else{
                Console.WriteLine("Der er ikke nogen bøger i det her bibliotek :(");
            }
            Console.WriteLine("=======================================");
            Console.WriteLine("Tryk en tast for at gå tilbage");
            Console.ReadKey(true);
        }

        public void ShowUsers()
        {
            Console.Clear();
            Console.WriteLine("\n=======================================\n=======================================");
            if(this.users.Count > 0){
                foreach(User user in this.users.Values){
                    Console.WriteLine(user.name);
                    Console.WriteLine(user.id);
                    Console.WriteLine(user.borrowedCount);
                    Console.WriteLine(user.passHashed);
                    Console.WriteLine("=======================================");
                }
            }else{
                Console.WriteLine("Der er ikke nogen brugere :(");
            }
            Console.WriteLine("\n=======================================\n=======================================");
            Console.WriteLine("Tryk en tast for at gå tilbage");
            Console.ReadKey(true);
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
            if(File.Exists("books.dat")){
            Stream streambooks = new FileStream("books.dat", FileMode.Open, FileAccess.Read);
            formatter.Serialize(streambooks, books);
            streambooks.Close();
            }

            if(File.Exists("users.dat")){
            Stream streamusers = new FileStream("users.dat", FileMode.Open, FileAccess.Read);
            formatter.Serialize(streamusers, books);
            streamusers.Close();
            }
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
