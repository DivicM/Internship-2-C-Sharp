var Members = new Dictionary<int, (string Name, string Surname, DateTime DateOfBirth, List<(int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn)> Loans)>
{
     { 1, ("Ivo", "Ivić", new DateTime(2000, 5, 1), new()
        {
            (1, "Na Drini ćuprija", "Ivo Andrić", new DateTime(2026, 10, 20), new DateTime(2026, 10, 30)),
            (2, "Zločin i kazna", "Fjodor Dostojevski", new DateTime(2026, 11, 1), null)
        })
    },
    { 2, ("Ana", "Anić", new DateTime(1998, 11, 23), new()
        {
            (3, "Gospodar prstenova", "J. R. R. Tolkien", new DateTime(2026, 9, 15), new DateTime(2026, 10, 10))
        })
    },
    { 3, ("Marko", "Marić", new DateTime(2003, 2, 14), new()) }
};

int lastMemberID = 3;

static int DaysLate(DateTime dateBorrowed, DateTime? dateReturned)
{
    DateTime DueDate = dateBorrowed.Date.AddDays(21);
    DateTime endDate = (dateReturned ?? DateTime.Today).Date;

    return Math.Max(0, (endDate - DueDate).Days);
}

static double LateFee(int daysLate)
{
    return daysLate * 0.5;
}

void addMember()
{
    int ID = ++lastMemberID;

    Console.WriteLine("Unesi ime korisnika");
    string userName = Console.ReadLine();
    while(string.IsNullOrWhiteSpace(userName))
    {
        Console.WriteLine("Krivi unos, unesi ime ponovo: ");
        userName = Console.ReadLine();
    }
    userName = userName.Trim();

    Console.WriteLine("Unesi prezime korisnika");
    string userSurname = Console.ReadLine();
    while (string.IsNullOrWhiteSpace(userSurname))
    {
        Console.WriteLine("Krivi unos, unesi prezime ponovo: ");
        userSurname = Console.ReadLine();
    }
    userSurname = userSurname.Trim();

    Console.WriteLine("Unesi datum rođenja (DD.MM.YYYY)");
    DateTime date;
    while (!DateTime.TryParse(Console.ReadLine().Trim(), out date) || date > DateTime.Today)
        Console.WriteLine("Neispravan unos, unesi ponovo: ");


    Members.Add(ID, (userName, userSurname, date, new()));

    Console.WriteLine($"Dodan korisnik {userName} {userSurname} rođen {date:dd:MM:yyyy}");
    

}

void MembersMenu()
{
    Console.WriteLine("1 - Unos novog člana\n2 - Brisanje Člana\n3 - Uređivanje člana\n4 - Pregled svih članova\n0 - Povratak na galvni izbornik");
    string answer = Console.ReadLine();

    switch (answer.Trim())
    {
        case "1":
            addMember();
            break;
        case "2":

        case "3":

        case "4":

        case "0":
            return;
    }
        

}


while(true)
{
    Console.WriteLine("1 - Članovi\n 2 - Posudbe\n 0 - Izlaz iz aplikacije");
    string answer = Console.ReadLine();

    switch (answer.Trim())
    {
        case "1":

        case "2":

        case "0":
            return;
        default: Console.WriteLine("Neispravan unos"); break;
    }
}
    









