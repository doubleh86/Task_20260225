using Task_20260225.Models;

namespace Task_20260225.Common.Services;

public class ContactCacheService
{
    private readonly List<ContactModel> _contactModels = [];

    public int AddContactList(List<ContactModel> contactModels)
    {
        _contactModels.AddRange(contactModels);
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
