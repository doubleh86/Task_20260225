using Task_20260225.Command;
using Task_20260225.Handlers.Commands;
using Task_20260225.Models;
using Task_20260225.Services;
using Task_20260225.Utils;

namespace TaskTest;

[TestFixture]
public class UploadEmployeeHandlerTest
{
    [Test]
    public async Task HandleAsync_WithJsonText_AddsContacts()
    {
        var cache = new ContactCacheService();
        var json = await File.ReadAllTextAsync(GetTestDataPath("contact.json"));
        var command = new UploadEmployeeInfoCommand(json);
        using var sut = new UploadEmployeeInfoHandler(command, cache);

        var result = await sut.HandleAsync();

        Assert.That(result, Is.EqualTo(50));
        var contacts = cache.GetContactList(1, 10);
        Assert.That(contacts.Count, Is.EqualTo(10));
        Assert.That(contacts[0].Name, Is.EqualTo("김철수"));
    }

    [Test]
    public async Task HandleAsync_WithCsvText_AddsContacts()
    {
        var cache = new ContactCacheService();
        var csv = await File.ReadAllTextAsync(GetTestDataPath("contact.csv"));
        var command = new UploadEmployeeInfoCommand(csv);
        using var sut = new UploadEmployeeInfoHandler(command, cache);

        var result = await sut.HandleAsync();

        Assert.That(result, Is.EqualTo(50));
        var contacts = cache.GetContactList(1, 10);
        Assert.That(contacts.Count, Is.EqualTo(10));
        Assert.That(contacts[0].Date, Is.EqualTo("2025.09.12"));
    }

    [Test]
    public void HandleAsync_WithInvalidCsvText_ThrowsServerException()
    {
        var cache = new ContactCacheService();
        var invalidCsv = "Kim, kim@test.com";
        var command = new UploadEmployeeInfoCommand(invalidCsv);
        using var sut = new UploadEmployeeInfoHandler(command, cache);

        Assert.ThrowsAsync<ServerException>(async () => await sut.HandleAsync());
    }

    [Test]
    public async Task HandleAsync_WithDuplicateEmail_SkipsDuplicate()
    {
        var cache = new ContactCacheService();
        cache.AddContactList([
            new ContactModel
            {
                Name = "기존사용자",
                Email = "dup@test.com",
                Phone = "010-0000-0000",
                Date = "2026.02.25"
            }
        ]);

        var json = """
                   [{"name":"신규사용자","email":"DUP@test.com","phone":"010-1111-2222","date":"2026.02.25"}]
                   """;
        var command = new UploadEmployeeInfoCommand(json);
        using var sut = new UploadEmployeeInfoHandler(command, cache);

        var result = await sut.HandleAsync();

        Assert.That(result, Is.EqualTo(1));
        var contacts = cache.GetContactList(1, 10);
        Assert.That(contacts.Count, Is.EqualTo(1));
        Assert.That(contacts[0].Name, Is.EqualTo("기존사용자"));
    }

    private static string GetTestDataPath(string fileName)
    {
        return Path.Combine(AppContext.BaseDirectory, "TestData", fileName);
    }
}
