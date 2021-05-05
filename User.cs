using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Text;



namespace biblo
{
    // maker til compileren at klassen skal kunne serializes
    [Serializable]
    public class User
    {
        public string name;
        public string passHashed;
        public int id;
        public int borrowedCount;
        // dictionary til at holde på de bøger som brugeren har lånt
        private Dictionary<int, Book> booksBorrowed = new Dictionary<int, Book>();

        public User(string name,string passHashed, int id, int borrowedCount){
            this.name = name;
            this.passHashed = passHashed;
            this.id = id;
            this.borrowedCount = borrowedCount;
        }
        // tjek om brugeren har lånt bogen
        public bool HasBook(int isbn){
            if(this.booksBorrowed.ContainsKey(isbn))
                return true;
            else
                return false;
        }

        // laver en ny bog hvis den bog brugeren har lånt ikke allerede er lånt,
        // derved kan brugeren låne flere bøger af samme slags
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

        // returnere en ny bog og remover den lånte bog hvis antallet kommer under 1
        public Book Return(int isbn){
            Book returnBook = new Book(
                    booksBorrowed[isbn].title,
                    booksBorrowed[isbn].author,
                    booksBorrowed[isbn].publisher,
                    booksBorrowed[isbn].genre,
                    booksBorrowed[isbn].published,
                    booksBorrowed[isbn].pages,
                    booksBorrowed[isbn].isbn);
            this.borrowedCount--;

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
}

