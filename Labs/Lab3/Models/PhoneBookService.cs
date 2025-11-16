using System;

namespace Lab3.Models;

public class PhoneBookService
{
    static int ID_COUNTER = 0;
    private static Random random = new Random();

    private Dictionary<int, Contact> contacts;

    public PhoneBookService()
    {
        contacts = new();
        for (int i = 0; i < 10; i++)
        {
            var contact = CreateRandomContact();
            contacts.Add(contact.Id, contact);
        }
    }

    public IEnumerable<Contact> GetContacts()
    {
        return contacts.Values;
    }
    
    public void Add(Contact contact)
    {
        int id = ID_COUNTER++;
        contact.Id = id;
        contacts.Add(id, contact);
    }
    
    public bool Remove(int id)
    {
        return contacts.Remove(id);
    }


    // Static factory method to create a list of random contacts
    protected static Contact CreateRandomContact()
    {
        var names = new List<string> { "Jan", "Adam", "Mariusz", "Jarosław", "James", "Rafał", "Kamil" };
        var surnames = new List<string> { "Kowalski", "Paździoch", "Nowak", "Kamiński", "Kaczmarek", "Nowicki", "Malinowski" };
        var cities = new List<string> { "Lublin", "Zamość", "Warszawa", "Kraków", "Poznań" };

        return new Contact
        {
            Id = ID_COUNTER++,
            Name = names[random.Next(names.Count)],
            Surname = surnames[random.Next(surnames.Count)],
            City = cities[random.Next(cities.Count)],
            Email = $"{names[random.Next(names.Count)].ToLower()}.{surnames[random.Next(surnames.Count)].ToLower()}@example.com",
            PhoneNumber = GenerateRandomPhoneNumber()
        };
    }

    private static string GenerateRandomPhoneNumber()
    {
        return random.Next(100000000, 999999999).ToString();
    }
}