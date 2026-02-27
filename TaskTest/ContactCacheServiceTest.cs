using Task_20260225.Common.Services;
using Task_20260225.Common.Utils;
using Task_20260225.Models;

namespace TaskTest;

[TestFixture]
[NonParallelizable]
public class ContactCacheServiceTest
{
    [Test]
    public void Initialize_WithUseTestDataFalse_DoesNotLoadData()
    {
        var cache = new ContactCacheService();
        var tempDir = CreateTempDirWithTestData();
        var originalDir = Directory.GetCurrentDirectory();

        try
        {
            Directory.SetCurrentDirectory(tempDir);

            cache.Initialize(null, useTestData: false);

            Assert.That(cache.GetContactCount(), Is.EqualTo(0));
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            Directory.Delete(tempDir, true);
        }
    }

    [Test]
    public void Initialize_WithUseTestDataTrue_LoadsData()
    {
        var cache = new ContactCacheService();
        var tempDir = CreateTempDirWithTestData();
        var originalDir = Directory.GetCurrentDirectory();

        try
        {
            Directory.SetCurrentDirectory(tempDir);

            cache.Initialize(null, useTestData: true);

            Assert.That(cache.GetContactCount(), Is.EqualTo(1));
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            Directory.Delete(tempDir, true);
        }
    }

    [Test]
    public void AddContactList_DeduplicatesByEmail_CaseInsensitive()
    {
        var cache = new ContactCacheService();

        var success = cache.AddContactList([
            new ContactModel { Name = "A", Email = "dup@test.com", Phone = "010-0000-0001", Date = "2026.01.01" },
            new ContactModel { Name = "B", Email = "DUP@test.com", Phone = "010-0000-0002", Date = "2026.01.02" }
        ], out var failed);

        Assert.That(success, Is.EqualTo(1));
        Assert.That(failed, Is.EqualTo(1));
        Assert.That(cache.GetContactCount(), Is.EqualTo(1));
    }

    [Test]
    public void AddContactList_WithInvalidEmail_ThrowsServerException()
    {
        var cache = new ContactCacheService();

        var ex = Assert.Throws<ServerException>(() => cache.AddContactList([
            new ContactModel { Name = "A", Email = "not-email", Phone = "010-0000-0001", Date = "2026-01-01" }
        ], out var _));

        Assert.That(ex?.ResultCode, Is.EqualTo(ErrorCode.ContactInvalidEmailFormat));
    }

    [Test]
    public void AddContactList_WithInvalidDate_ThrowsServerException()
    {
        var cache = new ContactCacheService();

        var ex = Assert.Throws<ServerException>(() => cache.AddContactList([
            new ContactModel { Name = "A", Email = "a@test.com", Phone = "010-0000-0001", Date = "2026/01/01" }
        ], out var _));

        Assert.That(ex?.ResultCode, Is.EqualTo(ErrorCode.ContactInvalidDateFormat));
    }

    private static string CreateTempDirWithTestData()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "Task_20260225_" + Guid.NewGuid().ToString("N"));
        var testDataDir = Path.Combine(tempDir, "TestData");
        Directory.CreateDirectory(testDataDir);

        var json = """
                   [
                     {
                       "name": "Seed User",
                       "email": "seed@test.com",
                       "phone": "010-0000-0000",
                       "date": "2026.02.26"
                     }
                   ]
                   """;
        File.WriteAllText(Path.Combine(testDataDir, "contact.json"), json);

        return tempDir;
    }
}
