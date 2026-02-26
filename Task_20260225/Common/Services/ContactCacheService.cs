using System.Text.Json;
using Task_20260225.Models;

namespace Task_20260225.Common.Services;

public class ContactCacheService
{
    private readonly List<ContactModel> _contactModels = [];
    private readonly Lock _sync = new(); // .Net 10 이라 가능

    public void Initialize(bool useTestData = false)
    {
        if (useTestData == false)
            return;

        var testDataPath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "contact.json");
        if (File.Exists(testDataPath) == false)
            return;

        try
        {
            var json = File.ReadAllText(testDataPath);
            var contacts = JsonSerializer.Deserialize<List<ContactModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (contacts is null || contacts.Count == 0)
                return;

            AddContactList(contacts);
        }
        catch (Exception)
        {
            // Ignore initialization errors to keep service startup resilient.
        }
    }

    public int AddContactList(List<ContactModel> contactModels)
    {
        lock (_sync)
        {
            if (contactModels.Count == 0)
                return _contactModels.Count;

            var existingEmails = new HashSet<string>(
                _contactModels
                    .Where(x => string.IsNullOrWhiteSpace(x.Email) == false)
                    .Select(x => x.Email.Trim()),
                StringComparer.OrdinalIgnoreCase);

            var uniqueContacts = contactModels.Where(contact =>
            {
                if (string.IsNullOrWhiteSpace(contact.Email))
                    return true;

                return existingEmails.Add(contact.Email.Trim());
            }).ToList();

            _contactModels.AddRange(uniqueContacts);
            return _contactModels.Count;
        }
    }

    public List<ContactModel> GetContactList(int page, int pageSize)
    {
        lock (_sync)
        {
            return _contactModels.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
    }

    public int GetContactCount()
    {
        lock (_sync)
        {
            return _contactModels.Count;
        }
    }

    public List<ContactModel> GetContactListByName(string name)
    {
        lock (_sync)
        {
            return _contactModels
                .Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
