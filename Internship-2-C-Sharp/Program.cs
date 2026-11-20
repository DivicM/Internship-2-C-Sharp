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

    Console.WriteLine($"Dodan korisnik {userName} {userSurname} rođen {date:dd:MM:yyyy}");
    Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
    Console.ReadKey();
}
void RemoveMemberByID()
{
    Console.WriteLine("Unesi ID korisnika:");
    if(!int.TryParse(Console.ReadLine(), out int id) || !Members.ContainsKey(id))
    {
        Console.WriteLine("Član s tim ID-jem ne postoji");
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
        return;
    }

    ConfirmAndRemove(id);

}

void ConfirmAndRemove(int ID)
{
    var member = Members[ID];

    Console.WriteLine($"Jeste li sigurni da zelite obristai {member.Name} {member.Surname} iz baze? DA/NE");
    string input = Console.ReadLine().Trim();
    if (input.ToLower() == "da" )
    {
        Members.Remove(ID);
        Console.WriteLine($"Korisnik {member.Name} {member.Surname} uklonjen.");
        Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
        Console.ReadKey();
    }
    else
    {
        Console.WriteLine("Brisanje otkazano");
        Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
        Console.ReadKey();
    }
    

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

        default: Console.WriteLine("Neispravan unos"); break;
    }

}

void EditMember()
{
    Console.WriteLine("Unesi ID korisnika");
    if(!int.TryParse(Console.ReadLine(), out int id) || !Members.ContainsKey(id))
    {
        Console.WriteLine("Krivi unos ID-ja");
        return;
    }
    var member = Members[id];

    Console.WriteLine($"Uređuješ: {member.Name} {member.Surname}, rođen {member.DateOfBirth:dd.MM.yyyy}");
    Console.WriteLine("Želite li uređivati: 1 - Ime\n2 - Prezime\n3 - Datum rođenja\n0 - Odustani");
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
            Console.WriteLine($"Datum promijenjen u {member.DateOfBirth:dd:MM:yyyy}");
            break;

        case "0":
            Console.WriteLine("Nema nikakve izmjene");
            return;
        default:
            Console.WriteLine("Neispravan unos"); return;

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
        Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
        Console.ReadKey();
        return;
    }
    Console.WriteLine($"{"ID",-5}{"Prezime",-15}{"Ime",-15}{"Godine",-8}{"Broj posudbi"}");

    foreach (var (id, member) in Members)
    {
        Console.WriteLine($"{id,-5}{member.Surname,-15}{member.Name,-15}{CalculateAge(member.DateOfBirth),-8}{member.Loans.Count}");
    }
    Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
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
        if (member.Loans.Count > 0)
        {
            count++;
        }
    }
    if (count == 0)
    {
        Console.WriteLine("Nema korisnika koji su posudili barem jednu knjigu");
        Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
        Console.ReadKey();
        return;
    }
    Console.WriteLine("Članovi koji imaju barem jednu posuđenu knjigu");
    Console.WriteLine($"{"ID",-5}{"Prezime",-15}{"Ime",-15}{"Godine",-8}{"Broj posudbi"}");

    foreach (var (id, member) in Members)
    {
        if (member.Loans.Count() != 0)
        {
            Console.WriteLine($"{id,-5}{member.Surname,-15}{member.Name,-15}{CalculateAge(member.DateOfBirth),-8}{member.Loans.Count}");
        }
    }
    Console.WriteLine("Pritisnite bilo koju tipku za izlazak");
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
        return;
    }
   
    switch(answer)
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
        default: Console.WriteLine("Neispravan unos"); break;

    }
}

void MembersMenu()
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
        default: Console.WriteLine("Neispravan unos"); break;
    }
        

}


while(true)
{
    Console.Clear();
    Console.WriteLine("1 - Članovi\n2 - Posudbe\n0 - Izlaz iz aplikacije");
    string answer = Console.ReadLine();
    
    switch (answer.Trim())
    {
        case "1":
            MembersMenu();
            break;
        case "2":

        case "0":
            return;
        default: Console.WriteLine("Neispravan unos"); break;
    }
}
    









