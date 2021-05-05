using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Text;



namespace biblo
{

    public class Library
    {
        // opretter dictionary private så kun library arbejder med dem direkte
        private Dictionary<int, Book> books = new Dictionary<int, Book>();
        private Dictionary<int, User> users = new Dictionary<int, User>();

        // tager bruger id og en string pass som er hashed og ser om brugerns kode passer til det indtastede
        public bool CheckPass(int id, string pass){
            if(this.users[id].passHashed == pass)
                return true;
            else
                return false;
        }

        // hvis brugeren har den bog han prøver at aflevere
        public void ReturnBook(int id, int isbn){
            if(users[id].HasBook(isbn)){
                if(this.books.ContainsKey(isbn)){
                    this.books[isbn].incCount();
                }else{
                    this.books.Add(isbn, this.users[id].Return(isbn));
                }
            }
        }

        // for at interface med users går man igennem libarary her til userens antal af bøger lånt
        public void ShowUserBorrowed(int id){
            users[id].ShowBorrowed();
        }

        public bool BookExists(int isbn){
            if(this.books.ContainsKey(isbn))
                return true;
            else
                return false;
        }

        // en bruger kalder denne metode for at låne
        // hvis bogen eksistere kaldes brugerens Borrow metode med den Book som er valgt som argument
        // derefter fjernes én bogen fra bibliotekets dictionary
        // metoden returnere om det var muligt at låne bogen
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

        // Har skrevet en del om getHighestID i dokumentation
        // foreach igennem alle brugere og sætter highID variablen til user.id hvis user.id er større.
        public int getHighestID(){
            int highID = 0;
                foreach(User user in this.users.Values){
                    if(highID < user.id)
                        highID = user.id;
                }
            return highID;
        }

        // add new user til Dictionary users
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

        // får en masse input fra en admin til at oprette en bog
        // hvis man opretter en bog med isbn som allrede eksistere vil den bare kalde incCount
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

        // Fjerner en bog hvis bogcount er 0 fjernes hele objektet
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
                    Console.WriteLine("Brugernavn: {0}", user.name);
                    Console.WriteLine("Bruger ID: {0}", user.id);
                    Console.WriteLine("Antal bøger lånt: {0}", user.borrowedCount);
                    Console.WriteLine("sha256 kode: {0}", user.passHashed);
                    Console.WriteLine("=======================================");
                }
            }else{
                Console.WriteLine("Der er ikke nogen brugere :(");
            }
            Console.WriteLine("\n=======================================\n=======================================");
            Console.WriteLine("Tryk en tast for at gå tilbage");
            Console.ReadKey(true);
        }

        // Gemmer dictionary users og books
        public void Export(){
            // laver en filestream til filen books.dat i samme mappe med write access
            FileStream fs = new FileStream("books.dat", FileMode.Create, FileAccess.Write, FileShare.None);
            // opretter en binaryformatter
            BinaryFormatter bf = new BinaryFormatter();
            // serializer books dictionary til binær data i filestreamen fs
            bf.Serialize(fs, this.books);
            // luk filestream
            fs.Close();

            // gør det samme for users dictionary'en
            FileStream fsUser = new FileStream("users.dat", FileMode.Create, FileAccess.Write, FileShare.None);
            bf.Serialize(fsUser, this.users);
            fsUser.Close();

        }
        // Loader dictionaries fra filerne books.dat & users.dat
        // checker om filerne findes så den ikke crasher hvis der ikke er nogen fil
        // ellers gør den næsten det samme som export bare binaryformatteren deserializer filestreamen
        // derefter caster jeg filestreamen til (Dictionary<int,User>)
        public void Import(){

            if(File.Exists("books.dat")){
            FileStream fs = new FileStream("books.dat", FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryFormatter bf = new BinaryFormatter();
            this.books = (Dictionary<int, Book>)bf.Deserialize(fs);
            fs.Close();
            }
            if(File.Exists("users.dat")){
            FileStream fs = new FileStream("users.dat", FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryFormatter bf = new BinaryFormatter();
            this.users = (Dictionary<int, User>)bf.Deserialize(fs);
            fs.Close();
            }
        }
    }
}

