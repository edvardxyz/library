using System;
using System.IO;

namespace biblo
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory<>
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

        public Book(string title, string author, string publisher, string genre, int published, int pages, int isbn){

            this.title = title;
            this.author = author;
            this.publisher = publisher;
            this.genre = genre;
            this.published = published;
            this.pages = pages;
            this.isbn = isbn;

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

