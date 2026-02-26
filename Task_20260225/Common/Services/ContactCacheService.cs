using System.Text.Json;
using System.Globalization;
using System.Net.Mail;
using Task_20260225.Models;

namespace Task_20260225.Common.Services;

public class ContactCacheService
{
    private readonly List<ContactModel> _contactModels = [];
    private readonly Lock _sync = new(); // .Net 10 이라 가능
    private LoggerService? _loggerService;

    public void Initialize(LoggerService? loggerService, bool useTestData = false)
    {
        _loggerService = loggerService;
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

            AddContactList(contacts, out var _);
        }
        catch (Exception)
        {
            // Ignore initialization errors to keep service startup resilient.
        }
    }

    public int AddContactList(List<ContactModel> contactModels, out int failed)
    {
        failed = 0;
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

            foreach (var contact in uniqueContacts)
            {
                if (_ValidateContact(contact) == false)
                {
                    failed += 1;
                    continue;
                }
                    
                _contactModels.Add(contact);
            }
            
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

    private bool _ValidateContact(ContactModel contact)
    {
        if (_IsValidEmail(contact.Email) == false)
        {
            _loggerService?.Information(this, $"Invalid email format [{contact.Email}]");
            return false;
        }

        if (_IsValidDate(contact.Date) == false)
        {
            _loggerService?.Information(this, $"Invalid date format [{contact.Date}]");
            return false;
        }

        return true;
    }

    private static bool _IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var parsed = new MailAddress(email.Trim());
            return string.Equals(parsed.Address, email.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool _IsValidDate(string? date)
    {
        if (string.IsNullOrWhiteSpace(date))
            return false;

        return DateTime.TryParseExact(
            date.Trim(),
            ["yyyy-MM-dd", "yyyy.MM.dd"],
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }
}
