using Task_20260225.Models;

namespace Task_20260225.Common.Services;

public class ContactCacheService
{
    private readonly List<ContactModel> _contactModels = [];

    public int AddContactList(List<ContactModel> contactModels)
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
        });

        _contactModels.AddRange(uniqueContacts);
        return _contactModels.Count;
    }

    public List<ContactModel> GetContactList(int page, int pageSize)
    {
        return _contactModels.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    public List<ContactModel> GetContactListByName(string name)
    {
        return _contactModels
            .Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
