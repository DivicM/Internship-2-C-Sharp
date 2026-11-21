Console.OutputEncoding = System.Text.Encoding.UTF8;

var Members = new Dictionary<int, (string Name, string Surname, DateTime DateOfBirth, List<(int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn)> Loans)>
{
    { 1, ("Ivo", "Ivić", new DateTime(2000, 5, 1), new()
        {
            (1, "Na Drini ćuprija", "Ivo Andrić", new DateTime(2026, 10, 20), new DateTime(2026, 10, 30)),
            (2, "Zločin i kazna", "Fjodor Dostojevski", new DateTime(2026, 11, 1), null),
            (4, "Proces", "Franz Kafka", new DateTime(2026, 9, 1), new DateTime(2026, 10, 5))
        })
    },
    { 2, ("Ana", "Anić", new DateTime(1998, 11, 23), new()
        {
            (3, "Gospodar prstenova", "J. R. R. Tolkien", new DateTime(2026, 9, 15), new DateTime(2026, 10, 10)),
            (5, "Mali princ", "Antoine de Saint-Exupéry", new DateTime(2026, 10, 15), null)
        })
    },
    { 3, ("Marko", "Marić", new DateTime(2003, 2, 14), new()
        {
            (6, "Ana Karenjina", "Lav Tolstoj", new DateTime(2026, 8, 10), new DateTime(2026, 8, 25)),
            (7, "Hobit", "J. R. R. Tolkien", new DateTime(2026, 11, 10), null)
        })
    },
    { 4, ("Luka", "Lukić", new DateTime(2012, 6, 10), new()) }
};

int lastMemberID = 4;
int lastRentID = 7;

static int CalculateDaysLate(DateTime dateBorrowed, DateTime? dateReturned)
{
    DateTime DueDate = dateBorrowed.Date.AddDays(21);
    DateTime endDate = (dateReturned ?? DateTime.Today).Date;

    return Math.Max(0, (endDate - DueDate).Days);
}

static double CalculateLateFee(int daysLate)
{
    return daysLate * 0.5;
}

static bool HasActiveLoans(List<(int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn)> loans)
{
    foreach (var loan in loans)
    {
        if (loan.DateOfReturn == null)
            return true;
    }
    return false;
}

static bool IsConfirmed(string question)
{
    Console.WriteLine($"{question} DA/NE");
    return Console.ReadLine()?.Trim().ToLower() == "da";
}

void AddMember()
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

    Console.WriteLine($"Dodan korisnik {userName} {userSurname} rođen {date:dd.MM.yyyy}");
    Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
    Console.ReadKey();
}
void RemoveMemberByID()
{
    Console.WriteLine("Unesi ID korisnika:");
    if(!int.TryParse(Console.ReadLine(), out int id) || !Members.ContainsKey(id))
    {
        Console.WriteLine("Član s tim ID-jem ne postoji");
        Console.ReadKey();
        return;
    }

    ConfirmAndRemove(id);


}
void RemoveMemberByName()
{
    Console.WriteLine("Unesite ime: ");
    string name = Console.ReadLine()?.Trim() ?? "";

    Console.WriteLine("Unesite prezime: ");
    string surname = Console.ReadLine()?.Trim() ?? "";
    int id=0;
    foreach(var m in Members)
    {
        if(m.Value.Name.ToLower()==name.ToLower() && m.Value.Surname.ToLower() == surname.ToLower())
        {
            id = m.Key;
            break;
        }
    }
    if (id == 0)
    {
        Console.WriteLine("Korisnik s tim imenom i prezimenom nije pronađen");
        Console.ReadKey();
        return;
    }

    ConfirmAndRemove(id);

}

void ConfirmAndRemove(int ID)
{
    var member = Members[ID];

    if (HasActiveLoans(member.Loans))
    {
        Console.WriteLine($"{member.Name} {member.Surname} ima nevraćenu knjigu i ne može se obrisati.");
        Console.WriteLine("Pritisnite bilo koju tipku za nastavak");
        Console.ReadKey();
        return;
    }

    Console.WriteLine($"Jeste li sigurni da želite obrisati {member.Name} {member.Surname} iz baze? DA/NE");
    string input = Console.ReadLine().Trim();
    if (input.ToLower() == "da")
    {
        Members.Remove(ID);
        Console.WriteLine($"Korisnik {member.Name} {member.Surname} uklonjen.");
    }
    else
    {
        Console.WriteLine("Brisanje otkazano");
    }

    Console.WriteLine("Pritisnite bilo koju tipku za nastavak");
    Console.ReadKey();
}
void RemoveMember()
{
    Console.WriteLine("Brisanje korisnika po:\n1 - ID-ju\n2 - Imenu i prezimenu\n0 - Povratak");
    string input = Console.ReadLine().Trim();

    switch (input)
    {
        case "1":
            RemoveMemberByID();
            break;
        case "2":
            RemoveMemberByName();
            break;
        case "0":
            return;

        default: 
            Console.WriteLine("Neispravan unos");
            Console.ReadKey();
            break;
    }

}

void EditMember()
{
    if (Members.Count == 0)
    {
        Console.WriteLine("Nema članova.");
        Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
        Console.ReadKey();
        return;
    }
    Console.WriteLine($"{"ID",-5}{"Prezime",-15}{"Ime",-15}{"Godine",-8}{"Broj posudbi"}");

    foreach (var (mid, m) in Members)
    {
        Console.WriteLine($"{mid,-5}{m.Surname,-15}{m.Name,-15}{CalculateAge(m.DateOfBirth),-8}{m.Loans.Count}");
    }
    Console.WriteLine("Unesi ID korisnika");
    if(!int.TryParse(Console.ReadLine(), out int id) || !Members.ContainsKey(id))
    {
        Console.WriteLine("Krivi unos ID-ja");
        Console.ReadKey();
        return;
    }
    var member = Members[id];

    Console.WriteLine($"Uređuješ: {member.Name} {member.Surname}, rođen {member.DateOfBirth:dd.MM.yyyy}");
    Console.WriteLine("Želite li uređivati:\n1 - Ime\n2 - Prezime\n3 - Datum rođenja\n0 - Odustani");
    string answer=Console.ReadLine().Trim();

    switch (answer)
    {
        case "1":
            Console.WriteLine("Unesi novo ime: ");
            string name = Console.ReadLine().Trim();
            while(string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Nije moguće unijeti prazan string");
                name = Console.ReadLine().Trim();
            }
            member.Name = name;
            Console.WriteLine($"Ime promijenjeno u {member.Name}");
            break;
        case "2":
            Console.WriteLine("Unesi novo prezime: ");
            string surname = Console.ReadLine().Trim();
            while (string.IsNullOrWhiteSpace(surname))
            {
                Console.WriteLine("Nije moguće unijeti prazan string");
                surname = Console.ReadLine().Trim();
            }

            member.Surname = surname;
            Console.WriteLine($"Prezime promijenjeno u {member.Surname}");
            break;
        case "3":
            Console.WriteLine("Unesi novi datum rođenja: (DD.MM.YYYY)");
            DateTime birth;
            while (!DateTime.TryParse(Console.ReadLine().Trim(), out birth) || birth > DateTime.Today)
            {
                Console.WriteLine("Neispravan unos!");
            }
            member.DateOfBirth= birth;
            Console.WriteLine($"Datum promijenjen u {member.DateOfBirth:dd.MM.yyyy}");
            break;

        case "0":
            Console.WriteLine("Nema nikakve izmjene");
            Console.ReadKey();
            return;
        default:
            Console.WriteLine("Neispravan unos");
            Console.ReadKey();
            return;

    }

    if (!IsConfirmed("Želite li spremiti izmjene?"))
    {
        Console.WriteLine("Izmjene odbačene.");
        Console.ReadKey();
        return;
    }

    Members[id] = member;
    Console.WriteLine($"{member.Name} {member.Surname}, rođen {member.DateOfBirth:dd.MM.yyyy}");
    Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
    Console.ReadKey();


}

static int CalculateAge(DateTime dateOfBirth)
{
    DateTime today = DateTime.Today;
    int age = today.Year - dateOfBirth.Year;

    if (dateOfBirth.Date > today.AddYears(-age))
        age--;

    return age;
}

void PrintAllMembers()
{
    if (Members.Count == 0)
    {
        Console.WriteLine("Nema članova.");
        Console.WriteLine("Pritisnite bilo koju tipku za nastavak");
        Console.ReadKey();
        return;
    }

    var ids = new List<int>(Members.Keys);
    ids.Sort((a, b) => Members[a].Surname.CompareTo(Members[b].Surname));

    foreach (int id in ids)
    {
        var m = Members[id];
        Console.WriteLine($"[{id}] {m.Surname}, {m.Name} ({CalculateAge(m.DateOfBirth)}) - broj posudbi: {m.Loans.Count}");
    }

    Console.WriteLine("Pritisnite bilo koju tipku za nastavak");
    Console.ReadKey();
}

void PrintUnder18()
{
    int count = 0;
    foreach (var (id, member) in Members)
    {
        if (CalculateAge(member.DateOfBirth) < 18)
        {
            count++;
        }
    }

    if (count == 0)
    {
        Console.WriteLine("Nema korisnika koji su mlađi od 18");
        Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
        Console.ReadKey();
        return;
    }
    
    Console.WriteLine("Članovi ispod 18 godina.");
    Console.WriteLine($"{"ID",-5}{"Prezime",-15}{"Ime",-15}{"Godine",-8}{"Broj posudbi"}");


    foreach (var (id, member) in Members)
    {
        if(CalculateAge(member.DateOfBirth)<18)
        {
            Console.WriteLine($"{id,-5}{member.Surname,-15}{member.Name,-15}{CalculateAge(member.DateOfBirth),-8}{member.Loans.Count}");
        }
    }
    Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
    Console.ReadKey();
}

void PrintHaveBorrowed()
{
    int count = 0;

    foreach (var (id, member) in Members)
    {
        int unreturned = 0;
        foreach (var loan in member.Loans)
        {
            if (loan.DateOfReturn == null)
                unreturned++;
        }

        if (unreturned > 0)
        {
            if (count == 0)
            {
                Console.WriteLine("Članovi koji imaju barem jednu nevraćenu knjigu");
                Console.WriteLine($"{"ID",-5}{"Prezime",-15}{"Ime",-15}{"Godine",-8}{"Nevraćeno"}");
            }

            count++;
            Console.WriteLine($"{id,-5}{member.Surname,-15}{member.Name,-15}{CalculateAge(member.DateOfBirth),-8}{unreturned}");
        }
    }

    if (count == 0)
        Console.WriteLine("Nema članova s nevraćenim knjigama.");

    Console.WriteLine("Pritisnite bilo koju tipku za nastavak");
    Console.ReadKey();
}

void PrintAll()
{
    Console.Clear();
    Console.WriteLine("1 - Ispis svih članova\n2 - Ispis svih mlađih od 18\n3 - Ispis svih koji imaju barem jednu nevraćenu knjigu\n0 - Povratak");
    string answer = Console.ReadLine();
    while (string.IsNullOrWhiteSpace(answer))
    {
        Console.WriteLine("Neispravan unos!");
        Console.ReadKey();
        return;
    }
   
    switch(answer.Trim())
    {
        case "1":
            PrintAllMembers();
            break;
        case "2":
            PrintUnder18();
            break;
        case "3":
            PrintHaveBorrowed();
            break;
        case "0":
            return;
        default: 
            Console.WriteLine("Neispravan unos");
            Console.ReadKey();
            break;

    }
}

void ShowMembersMenu()
{
    Console.Clear();
    Console.WriteLine("1 - Unos novog člana\n2 - Brisanje Člana\n3 - Uređivanje člana\n4 - Pregled svih članova\n0 - Povratak na galvni izbornik");
    string answer = Console.ReadLine();

    switch (answer.Trim())
    {
        case "1":
            AddMember();
            break;
        case "2":
            RemoveMember();
            break;
        case "3":
            EditMember();
            break;
        case "4":
            PrintAll();
            break;
        case "0":
            return;
        default: 
            Console.WriteLine("Neispravan unos");
            Console.ReadKey();
            break;
    }
        

}


void AddRent()
{
    Console.WriteLine($"{"ID",-5}{"Prezime",-15}{"Ime",-15}{"Godine",-8}{"Broj posudbi"}");

    foreach (var (id, m) in Members)
    {
        Console.WriteLine($"{id,-5}{m.Surname,-15}{m.Name,-15}{CalculateAge(m.DateOfBirth),-8}{m.Loans.Count}");
    }
    Console.WriteLine("Upiši ID korisnika kojemu želite dodati novu posudbu.");
    if(!int.TryParse(Console.ReadLine(), out int inputID) || !Members.ContainsKey(inputID))
    {
        Console.WriteLine("Neispravan unos");
        Console.ReadKey();
        return;  
    }

    var member=Members[inputID];

    Console.WriteLine("Unesi naslov knjige");
    string bookName = Console.ReadLine().Trim();
    while(string.IsNullOrWhiteSpace(bookName))
    {
        Console.WriteLine("Neispravan unos");
        bookName = Console.ReadLine().Trim();
    }

    Console.WriteLine("Unesi autora knjige");
    string bookAuthor = Console.ReadLine().Trim();
    while(string.IsNullOrWhiteSpace(bookAuthor))
    {
        Console.WriteLine("Neispravan unos");
        bookAuthor = Console.ReadLine().Trim();
    }

    Console.WriteLine("Unesi datum posudbe (DD.MM.YYYY)");
    DateTime dateRent;
    string input = Console.ReadLine().Trim();
    while(!DateTime.TryParse(input, out dateRent) || dateRent > DateTime.Today)
    {
        Console.WriteLine("Krivi unos, unesi ponovo");
        input = Console.ReadLine();
        
    }

    lastRentID++;
    
    member.Loans.Add((lastRentID,bookName, bookAuthor, dateRent, null));

    Console.WriteLine($"Korisnik mora vratiti knjigu do {dateRent.AddDays(21):dd.MM.yyyy}");
    Console.ReadKey();

}

void ReturnBook()
{

    Console.WriteLine($"{"ID",-5}{"Prezime",-15}{"Ime",-15}{"Godine",-8}{"Broj posudbi"}");

    foreach (var (id, m) in Members)
    {
        Console.WriteLine($"{id,-5}{m.Surname,-15}{m.Name,-15}{CalculateAge(m.DateOfBirth),-8}{m.Loans.Count}");
    }
    Console.WriteLine("Upiši ID korisnika kojemu želite ispuniti povratak knjige.");
    if (!int.TryParse(Console.ReadLine(), out int inputID) || !Members.ContainsKey(inputID))
    {
        Console.WriteLine("Neispravan unos");
        Console.ReadKey();
        return;
    }

    var member = Members[inputID];
    if (member.Loans.Count == 0)
    {
        Console.WriteLine("Član nema posudbi.");
        Console.ReadKey();
        return;
    }

    Console.WriteLine($"{"ID",-5}{"Naslov",-25}{"Autor",-22}{"Posuđeno",-12}{"Vraćeno",-14}{"Kašnjenje",-11}{"Zakasnina"}");

    foreach (var rent in member.Loans)
    {
        string returned = rent.DateOfReturn?.ToString("dd.MM.yyyy") ?? "nije vraćeno";
        int daysLate = CalculateDaysLate(rent.DateBorrowed, rent.DateOfReturn);

        Console.WriteLine($"{rent.ID,-5}{rent.Title,-25}{rent.Author,-22}{rent.DateBorrowed,-12:dd.MM.yyyy}{returned,-14}{daysLate,-11}{CalculateLateFee(daysLate):F2} €");
    }

    Console.WriteLine("Unesi ID posudbe:");
    int loanIndex = -1;

    while (loanIndex == -1)
    {
        if (int.TryParse(Console.ReadLine(), out int bookID))
        {
            for (int i = 0; i < member.Loans.Count; i++)
            {
                if (member.Loans[i].ID == bookID)
                {
                    loanIndex = i;
                    break;
                }
            }
        }
        if (loanIndex == -1)
            Console.WriteLine("Neispravan unos, unesi ponovo:");
    }
    var loan = member.Loans[loanIndex];

    if (loan.DateOfReturn != null)
    {
        Console.WriteLine($"Knjiga je vec vracena {loan.DateOfReturn.Value:dd.MM.yyyy}");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("Unesi datum povratka knjige (DD.MM.YYYY)");
    string inputDate=Console.ReadLine().Trim();
    DateTime returnDate;
    while(!DateTime.TryParse(inputDate, out returnDate) || returnDate > DateTime.Today || returnDate < loan.DateBorrowed.Date)
    {
        Console.WriteLine("Neispravan unos");
        inputDate = Console.ReadLine().Trim();
    }

    member.Loans[loanIndex] = (loan.ID, loan.Title,loan.Author, loan.DateBorrowed, returnDate);
    int lateDays = CalculateDaysLate(loan.DateBorrowed, returnDate);

    Console.WriteLine($"Knjiga vraćena. Dana kašnjenja: {lateDays}, zakasnina: {CalculateLateFee(lateDays):F2} €");
    Console.WriteLine("Pritisnite bilo koju tipku za nastavak");
    Console.ReadKey();
}


void DeleteRent()
{
    Console.WriteLine($"{"ID",-5}{"Prezime",-15}{"Ime",-15}{"Godine",-8}{"Broj posudbi"}");

    foreach (var (id, m) in Members)
    {
        Console.WriteLine($"{id,-5}{m.Surname,-15}{m.Name,-15}{CalculateAge(m.DateOfBirth),-8}{m.Loans.Count}");
    }
    Console.WriteLine("Upiši ID korisnika kojemu želite izbrisati posudbu.");
    if (!int.TryParse(Console.ReadLine(), out int inputID) || !Members.ContainsKey(inputID))
    {
        Console.WriteLine("Neispravan unos");
        Console.ReadKey();
        return;
    }

    var member = Members[inputID];
    if (member.Loans.Count == 0)
    {
        Console.WriteLine("Član nema posudbi.");
        Console.ReadKey();
        return;
    }

    Console.WriteLine($"{"ID",-5}{"Naslov",-25}{"Autor",-22}{"Posuđeno",-12}{"Vraćeno",-14}{"Kašnjenje",-11}{"Zakasnina"}");

    foreach (var rent in member.Loans)
    {
        string returned = rent.DateOfReturn?.ToString("dd.MM.yyyy") ?? "nije vraćeno";
        int daysLate = CalculateDaysLate(rent.DateBorrowed, rent.DateOfReturn);

        Console.WriteLine($"{rent.ID,-5}{rent.Title,-25}{rent.Author,-22}{rent.DateBorrowed,-12:dd.MM.yyyy}{returned,-14}{daysLate,-11}{CalculateLateFee(daysLate):F2} €");
    }

    Console.WriteLine("Unesi ID posudbe:");
    int loanIndex = -1;

    while (loanIndex == -1)
    {
        if (int.TryParse(Console.ReadLine(), out int bookID))
        {
            for (int i = 0; i < member.Loans.Count; i++)
            {
                if (member.Loans[i].ID == bookID)
                {
                    loanIndex = i;
                    break;
                }
            }
        }
        if (loanIndex == -1)
            Console.WriteLine("Neispravan unos, unesi ponovo:");
    }
    var loan = member.Loans[loanIndex];

    Console.WriteLine($"Jeste li sigurni da zelite izbrisati posudbu | {loan.Title}, {loan.Author} | DA/NE");
    if (Console.ReadLine()?.Trim().ToLower() == "da")
    {
        member.Loans.RemoveAt(loanIndex);
        Console.WriteLine("Posudba obrisana.");
    }
    else
    {
        Console.WriteLine("Brisanje otkazano.");
    }

    Console.WriteLine("Pritisnite bilo koju tipku za nastavak");
    Console.ReadKey();

}


void DeleteReturnedRents()
{
    Console.WriteLine($"{"ID",-5}{"Prezime",-15}{"Ime",-15}{"Godine",-8}{"Broj posudbi"}");
    foreach (var (id, m) in Members)
        Console.WriteLine($"{id,-5}{m.Surname,-15}{m.Name,-15}{CalculateAge(m.DateOfBirth),-8}{m.Loans.Count}");

    Console.WriteLine("Upiši ID člana kojemu želite izbrisati vraćene posudbe:");
    if (!int.TryParse(Console.ReadLine(), out int inputID) || !Members.ContainsKey(inputID))
    {
        Console.WriteLine("Neispravan unos");
        Console.ReadKey();
        return;
    }

    var loans = Members[inputID].Loans;

    if (!IsConfirmed("Želite li obrisati sve vraćene posudbe ovog člana?"))
    {
        Console.WriteLine("Brisanje otkazano.");
        Console.ReadKey();
        return;
    }

    int removed = 0;

    for (int i = loans.Count - 1; i >= 0; i--)
    {
        if (loans[i].DateOfReturn != null)
        {
            loans.RemoveAt(i);
            removed++;
        }
    }

    Console.WriteLine($"Obrisano vraćenih posudbi: {removed}");
    Console.WriteLine("Pritisnite bilo koju tipku za nastavak");
    Console.ReadKey();
}

void DeleteRentsByLateFee()
{
    Console.WriteLine("Unesi iznos zakasnine (2,50):");
    double amount;
    while (!double.TryParse(Console.ReadLine(), out amount) || amount < 0)
        Console.WriteLine("Neispravan iznos, unesi ponovo:");

    if (!IsConfirmed($"Želite li obrisati sve posudbe sa zakasninom većom od {amount:F2} €?"))
    {
        Console.WriteLine("Brisanje otkazano.");
        Console.ReadKey();
        return;
    }

    int removed = 0;
    foreach (var (id, m) in Members)
    {
        for (int i = m.Loans.Count - 1; i >= 0; i--)
        {
            var loan = m.Loans[i];
            if (CalculateLateFee(CalculateDaysLate(loan.DateBorrowed, loan.DateOfReturn)) > amount)
            {
                m.Loans.RemoveAt(i);
                removed++;
            }
        }
    }

    Console.WriteLine($"Obrisano posudbi sa zakasninom većom od {amount:F2} €: {removed}");
    Console.WriteLine("Pritisnite bilo koju tipku za nastavak");
    Console.ReadKey();
}

void ShowDeleteRentMenu()
{
    Console.Clear();
    Console.WriteLine("1 - Brisanje po ID-u posudbe\n2 - Brisanje svih vraćenih posudbi člana\n3 - Brisanje posudbi sa zakasninom većom od iznosa\n0 - Povratak");

    switch (Console.ReadLine()?.Trim())
    {
        case "1":
            DeleteRent();
            break;
        case "2":
            DeleteReturnedRents();
            break;
        case "3":
            DeleteRentsByLateFee();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Neispravan unos");
            Console.ReadKey();
            break;
    }
}


void EditRent()
{

    Console.WriteLine($"{"ID",-5}{"Prezime",-15}{"Ime",-15}{"Godine",-8}{"Broj posudbi"}");

    foreach (var (id, m) in Members)
    {
        Console.WriteLine($"{id,-5}{m.Surname,-15}{m.Name,-15}{CalculateAge(m.DateOfBirth),-8}{m.Loans.Count}");
    }
    Console.WriteLine("Upiši ID korisnika kojemu želite urediti posudbu.");
    if (!int.TryParse(Console.ReadLine(), out int inputID) || !Members.ContainsKey(inputID))
    {
        Console.WriteLine("Neispravan unos");
        Console.ReadKey();
        return;
    }

    var member = Members[inputID];
    if (member.Loans.Count == 0)
    {
        Console.WriteLine("Član nema posudbi.");
        Console.ReadKey();
        return;
    }

    Console.WriteLine($"{"ID",-5}{"Naslov",-25}{"Autor",-22}{"Posuđeno",-12}{"Vraćeno",-14}{"Kašnjenje",-11}{"Zakasnina"}");

    foreach (var rent in member.Loans)
    {
        string returned = rent.DateOfReturn?.ToString("dd.MM.yyyy") ?? "nije vraćeno";
        int daysLate = CalculateDaysLate(rent.DateBorrowed, rent.DateOfReturn);

        Console.WriteLine($"{rent.ID,-5}{rent.Title,-25}{rent.Author,-22}{rent.DateBorrowed,-12:dd.MM.yyyy}{returned,-14}{daysLate,-11}{CalculateLateFee(daysLate):F2} €");
    }

    Console.WriteLine("Unesi ID posudbe:");
    int loanIndex = -1;

    while (loanIndex == -1)
    {
        if (int.TryParse(Console.ReadLine(), out int bookID))
        {
            for (int i = 0; i < member.Loans.Count; i++)
            {
                if (member.Loans[i].ID == bookID)
                {
                    loanIndex = i;
                    break;
                }
            }
        }
        if (loanIndex == -1)
            Console.WriteLine("Neispravan unos, unesi ponovo:");
    }
    var loan = member.Loans[loanIndex];
    Console.WriteLine("Što želite urediti?\n1 - Naslov\n2 - Autor\n3 - Datum posudbe\n4 - Datum vraćanja\n0 - Odustani");

    switch (Console.ReadLine()?.Trim())
    {
        case "1":
            Console.WriteLine("Unesi novi naslov:");
            string title = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Naslov ne smije biti prazan, unesi ponovo:");
                title = Console.ReadLine();
            }
            loan.Title = title.Trim();
            break;

        case "2":
            Console.WriteLine("Unesi novog autora:");
            string author = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(author))
            {
                Console.WriteLine("Autor ne smije biti prazan, unesi ponovo:");
                author = Console.ReadLine();
            }
            loan.Author = author.Trim();
            break;

        case "3":
            DateTime latestBorrow = loan.DateOfReturn ?? DateTime.Today;
            Console.WriteLine("Unesi novi datum posudbe (dd.MM.yyyy):");
            DateTime newBorrowed;
            while (!DateTime.TryParse(Console.ReadLine(), out newBorrowed) || newBorrowed > latestBorrow)
                Console.WriteLine($"Neispravan datum, ne smije biti nakon {latestBorrow:dd.MM.yyyy}. Unesi ponovo:");
            loan.DateBorrowed = newBorrowed;
            break;

        case "4":
            Console.WriteLine("Unesi novi datum vraćanja (dd.MM.yyyy) ili prazno ako knjiga nije vraćena:");
            while (true)
            {
                string input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    loan.DateOfReturn = null;
                    break;
                }
                if (DateTime.TryParse(input, out DateTime newReturn) && newReturn >= loan.DateBorrowed.Date && newReturn <= DateTime.Today)
                {
                    loan.DateOfReturn = newReturn;
                    break;
                }
                Console.WriteLine($"Neispravan datum, mora biti između {loan.DateBorrowed:dd.MM.yyyy} i {DateTime.Today:dd.MM.yyyy}. Unesi ponovo:");
            }
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Neispravan unos");
            Console.ReadKey();
            return;
    }

    if (!IsConfirmed("Želite li spremiti izmjene?"))
    {
        Console.WriteLine("Izmjene odbačene.");
        Console.ReadKey();
        return;
    }


    member.Loans[loanIndex] = loan;



    int lateDays = CalculateDaysLate(loan.DateBorrowed, loan.DateOfReturn);
    Console.WriteLine($"Posudba ažurirana. Dana kašnjenja: {lateDays}, zakasnina: {CalculateLateFee(lateDays):F2} €");
    Console.WriteLine("Pritisnite bilo koju tipku za nastavak");
    Console.ReadKey();
}

List<(int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn)> GetAllRents()
{
    var all = new List<(int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn)>();
    foreach (var (id, m) in Members)
    {
        foreach (var loan in m.Loans)
            all.Add(loan);
    }
    return all;
}

static double CalculateRentFee((int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn) loan)
{
    return CalculateLateFee(CalculateDaysLate(loan.DateBorrowed, loan.DateOfReturn));
}

static string FormatRent((int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn) loan)
{
    string returned = loan.DateOfReturn?.ToString("yyyy-MM-dd") ?? "nije vraćeno";
    int daysLate = CalculateDaysLate(loan.DateBorrowed, loan.DateOfReturn);
    return $"[{loan.ID}] \"{loan.Title}\" - {loan.Author} | posuđeno: {loan.DateBorrowed:yyyy-MM-dd} | vraćeno: {returned} | kašnjenje: {daysLate} dana | zakasnina: {CalculateLateFee(daysLate):F2} €";
}

static void PrintRentList(List<(int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn)> rents)
{
    if (rents.Count == 0)
    {
        Console.WriteLine("Nema posudbi.");
        return;
    }
    foreach (var loan in rents)
        Console.WriteLine(FormatRent(loan));
}

static bool AskAscending()
{
    Console.WriteLine("1 - Uzlazno\n2 - Silazno");
    while (true)
    {
        string input = Console.ReadLine()?.Trim();
        if (input == "1") return true;
        if (input == "2") return false;
        Console.WriteLine("Neispravan unos, unesi 1 ili 2:");
    }
}

void PrintAllRents()
{
    PrintRentList(GetAllRents());
}

void PrintRentsByDate()
{
    var rents = GetAllRents();
    bool ascending = AskAscending();

    rents.Sort((a, b) => a.DateBorrowed.CompareTo(b.DateBorrowed));
    if (!ascending)
        rents.Reverse();

    PrintRentList(rents);
}

void PrintRentsByFee()
{
    var rents = GetAllRents();
    bool ascending = AskAscending();

    rents.Sort((a, b) => CalculateRentFee(a).CompareTo(CalculateRentFee(b)));
    if (!ascending)
        rents.Reverse();

    PrintRentList(rents);
}

void PrintUnreturnedRents()
{
    int count = 0;
    foreach (var (id, m) in Members)
    {
        foreach (var loan in m.Loans)
        {
            if (loan.DateOfReturn == null)
            {
                Console.WriteLine($"{FormatRent(loan)} | član: {m.Name} {m.Surname}");
                count++;
            }
        }
    }

    if (count == 0)
        Console.WriteLine("Nema nevraćenih posudbi.");
}

void SearchRents()
{
    Console.WriteLine("Unesi dio naslova ili autora:");
    string term = Console.ReadLine();
    while (string.IsNullOrWhiteSpace(term))
    {
        Console.WriteLine("Pojam ne smije biti prazan, unesi ponovo:");
        term = Console.ReadLine();
    }
    string termLower = term.Trim().ToLower();

    var found = new List<(int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn)>();
    foreach (var loan in GetAllRents())
    {
        if (loan.Title.ToLower().Contains(termLower) || loan.Author.ToLower().Contains(termLower))
            found.Add(loan);
    }

    PrintRentList(found);
}
void PrintRentsByMonth()
{
    var groups = new Dictionary<string, List<(int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn)>>();
    foreach (var loan in GetAllRents())
    {
        string month = loan.DateBorrowed.ToString("yyyy-MM");
        if (!groups.ContainsKey(month))
            groups[month] = new();
        groups[month].Add(loan);
    }

    if (groups.Count == 0)
    {
        Console.WriteLine("Nema posudbi.");
        return;
    }

    var months = new List<string>(groups.Keys);
    months.Sort();

    foreach (string month in months)
    {
        double totalFee = 0;
        foreach (var loan in groups[month])
            totalFee += CalculateRentFee(loan);

        Console.WriteLine($"\n{month} | broj posudbi: {groups[month].Count} | ukupna zakasnina: {totalFee:F2} €");
        foreach (var loan in groups[month])
            Console.WriteLine("   " + FormatRent(loan));
    }
}

void ShowPrintRentsMenu()
{
    Console.Clear();
    Console.WriteLine("1 - Sve posudbe\n2 - Sortirano po datumu posudbe\n3 - Sortirano po zakasnini\n4 - Nevraćene posudbe\n5 - Pretraga po naslovu ili autoru\n6 - Grupirano po mjesecu\n0 - Povratak");

    switch (Console.ReadLine()?.Trim())
    {
        case "1": 
            PrintAllRents(); 
            break;
        case "2": 
            PrintRentsByDate(); 
            break;
        case "3": 
            PrintRentsByFee(); 
            break;
        case "4": 
            PrintUnreturnedRents(); 
            break;
        case "5": 
            SearchRents(); 
            break;
        case "6": 
            PrintRentsByMonth(); 
            break;
        case "0": 
            return;
        default: Console.WriteLine("Neispravan unos"); break;
    }

    Console.WriteLine("\nPritisnite bilo koju tipku za nastavak");
    Console.ReadKey();
}

void PrintMemberSummary(List<(int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn)> loans)
{
    if (loans.Count == 0)
    {
        Console.WriteLine("Član nema posudbi.");
        return;
    }

    int unreturned = 0;
    double totalFee = 0;
    int returnedCount = 0;
    int totalReturnedDays = 0;
    int maxIndex = 0;
    int maxDaysLate = -1;

    for (int i = 0; i < loans.Count; i++)
    {
        var loan = loans[i];
        int daysLate = CalculateDaysLate(loan.DateBorrowed, loan.DateOfReturn);

        totalFee += CalculateLateFee(daysLate);

        if (loan.DateOfReturn == null)
        {
            unreturned++;
        }
        else
        {
            returnedCount++;
            totalReturnedDays += (loan.DateOfReturn.Value.Date - loan.DateBorrowed.Date).Days;
        }

        if (daysLate > maxDaysLate)
        {
            maxDaysLate = daysLate;
            maxIndex = i;
        }
    }

    Console.WriteLine($"Ukupno posudbi: {loans.Count}");
    Console.WriteLine($"Trenutno nevraćenih: {unreturned}");
    Console.WriteLine($"Ukupna zakasnina: {totalFee:F2} €");

    if (returnedCount > 0)
        Console.WriteLine($"Prosječno trajanje posudbe: {(double)totalReturnedDays / returnedCount:F1} dana");
    else
        Console.WriteLine("Prosječno trajanje posudbe: nema vraćenih posudbi");

    if (maxDaysLate > 0)
        Console.WriteLine($"Najveće kašnjenje: {FormatRent(loans[maxIndex])}");
    else
        Console.WriteLine("Najveće kašnjenje: nijedna posudba ne kasni");
}


void PrintLoansInRange(List<(int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn)> loans)
{
    Console.WriteLine("Unesi početni datum (dd.MM.yyyy):");
    DateTime from;
    while (!DateTime.TryParse(Console.ReadLine(), out from))
        Console.WriteLine("Neispravan datum, unesi ponovo:");

    Console.WriteLine("Unesi završni datum (dd.MM.yyyy):");
    DateTime to;
    while (!DateTime.TryParse(Console.ReadLine(), out to) || to < from)
        Console.WriteLine($"Neispravan datum, ne smije biti prije {from:dd.MM.yyyy}. Unesi ponovo:");

    var inRange = new List<(int ID, string Title, string Author, DateTime DateBorrowed, DateTime? DateOfReturn)>();
    foreach (var loan in loans)
    {
        if (loan.DateBorrowed.Date >= from.Date && loan.DateBorrowed.Date <= to.Date)
            inRange.Add(loan);
    }

    PrintRentList(inRange);
}


void ShowReportsMenu()
{
    Console.Clear();
    Console.WriteLine($"{"ID",-5}{"Prezime",-15}{"Ime",-15}{"Godine",-8}{"Broj posudbi"}");
    foreach (var (id, m) in Members)
        Console.WriteLine($"{id,-5}{m.Surname,-15}{m.Name,-15}{CalculateAge(m.DateOfBirth),-8}{m.Loans.Count}");

    Console.WriteLine("Upiši ID člana za izvještaj:");
    if (!int.TryParse(Console.ReadLine(), out int inputID) || !Members.ContainsKey(inputID))
    {
        Console.WriteLine("Neispravan unos");
        Console.ReadKey();
        return;
    }

    var member = Members[inputID];
    Console.WriteLine($"\nIzvještaj za: {member.Name} {member.Surname}");
    Console.WriteLine("1 - Sažetak posudbi\n2 - Posudbe u rasponu datuma\n0 - Povratak");

    switch (Console.ReadLine()?.Trim())
    {
        case "1": 
            PrintMemberSummary(member.Loans); 
            break;
        case "2": 
            PrintLoansInRange(member.Loans); 
            break;
        case "0": 
            return;
        default: Console.WriteLine("Neispravan unos"); break;
    }

    Console.WriteLine("\nPritisnite bilo koju tipku za nastavak");
    Console.ReadKey();
}

void ShowRentMenu()
{
    Console.Clear();
    Console.WriteLine("1 - Unos nove posudbe\n2 - Vraćanje knjige\n3 - Brisanje posudbe\n4 - Uređivanje postojeće posudbe\n5 - Pregled svih posudbi\n6 - Izvještaji i analize\n0 - Izlaz");
    string answer = Console.ReadLine();

    switch (answer.Trim())
    {
        case "1":
            AddRent();
            break;
        case "2":
            ReturnBook();
            break;
        case "3":
            ShowDeleteRentMenu();
            break;
        case "4":
            EditRent();
            break;
        case "5":
            ShowPrintRentsMenu();
            break;
        case "6":
            ShowReportsMenu();
            break;
        case "0":
            return;
        default: 
            Console.WriteLine("Neispravan unos");
            Console.ReadKey();
            break;
    }


}


while (true)
{
    Console.Clear();
    Console.WriteLine("1 - Članovi\n2 - Posudbe\n0 - Izlaz iz aplikacije");
    string answer = Console.ReadLine();
    
    switch (answer.Trim())
    {
        case "1":
            ShowMembersMenu();
            break;
        case "2":
            ShowRentMenu();
            break;
        case "0":
            return;
        default: 
            Console.WriteLine("Neispravan unos");
            Console.ReadKey();
            break;
    }
}
    









