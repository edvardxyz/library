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

    // interface klasse som styrer alle menuer og interaktioner mellem Library User og Book objekter
    public class Interface
    {

        public static void Run(){
            Library lib = new Library();
            // kalder lib.import for at hente evt. gemt data for users og books
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
                        // hvis brugeren eksitere giv brugeren 5 forsøg til at skrive koden korrekt
                        do{
                            if(lib.CheckPass(loginID, GetPass())){
                                login = true;
                                break;
                            }else{
                                Console.WriteLine("Forkert kode!\n{0} forsøg tilbage\nTryk enter for at prøve igen.\nEller Escape for at lukke",passTries);
                                // break hvis escape ellers forsæt til næste forsøg
                                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                    break;
                            }
                        }while(--passTries > 0);

                    }else{
                        Console.WriteLine("Bruger eksistere ikke\nTryk en tast for at fortsætte");
                        Console.ReadKey(true);
                    }
                    // hvis login true start UserMenu metode som taber Librariet og user IDet som argument
                    if(login)
                        UserMenu(lib, loginID);
                    // sæt login false når der logges ud
                    login = false;
                    // loop menuen
                    goto RunMenu;

                    // opret ny bruger kalder lib.Adduser metode inde i console.writeline da den returnere brugerens id
                case 2:
                    Console.Clear();
                    Console.WriteLine("\nDit bruger id er: {0}", lib.AddUser());
                    Console.WriteLine("Brug dit id til at logge ind");
                    Console.WriteLine("\nTryk for at gå tilbage");
                    Console.ReadKey(true);
                    goto RunMenu;

                    // admin login GetPass skal returnere en sha256 string det ligmed den hardcodede kode
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
                   // default burde aldrig kunne blive ramt
                default:
                    Console.WriteLine("Ikke en menu!");
                    Console.ReadKey(true);
                    goto RunMenu;
            }

            // når programmet slutter exporter users og books til deres fil
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

        // funktion til at lave stjerne GetPass
        public static string PrintStars(int starCount)
        {
            string stars = "";
            for(int i = 0; i < starCount; i++){
                stars += "*";
            }
            return stars;
        }

        public static string GetPass()
        {
            int starCount = 0;
            // tom string hvor hver char bliver appended
            string password = "";
            do
            {
                Console.Clear();
                Console.Write("Indtast kode: {0}", PrintStars(starCount));
                // tager keypress fra user putter ConsoleKeyInfo ind i keyinfo
                ConsoleKeyInfo keyinfo = Console.ReadKey(true);

                // hvis backspace fjern sidste char og fjern stjerne, hvis password.Length er 0 continue for at undgå crash
                if(keyinfo.Key == ConsoleKey.Backspace){
                    if(password.Length == 0)
                        continue;
                    password = password.Remove(password.Length-1);
                    starCount--;
                    continue;
                }
                starCount++;
                // hvis enter så break
                if(keyinfo.Key == ConsoleKey.Enter)
                    break;

                // append keyinfo lavet til char
                password += keyinfo.KeyChar;
            }while(true);
            // hasher stringen med pass.hash function og returere derefter
            return Pass.Hash(password);
        }
    }

    // laver SHA256 objekt
    public class Pass
    {
        public static string Hash(string passString)
        {
            string passHashString = "";
            SHA256 sha256obj = SHA256.Create();
            // SHA256 ComputeHash tager kun byte array, derfor konverteres string her;
            byte[] passBytes = sha256obj.ComputeHash(Encoding.UTF8.GetBytes(passString));

            // for loop igennem hver byte of lav til string med argument "x2"
            // x2 er format specifier så en byte altid bliver til 2char lang string
            for ( int i =0; i < passBytes.Length; i++){

                passHashString += passBytes[i].ToString("x2");
            }
            return passHashString;
        }
    }
}

