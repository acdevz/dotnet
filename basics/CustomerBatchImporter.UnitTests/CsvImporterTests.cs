using System.Runtime.CompilerServices;
using System.Text;
using FakeItEasy;

namespace CustomerBatchImporter.UnitTests;

public class CsvImporterTests // this object is created everytime the tests run
{
    private readonly ICustomerRepository _fakeCustomerRepo;
    private readonly CsvImporter _csvImporter;

    public CsvImporterTests()
    {
        _fakeCustomerRepo = A.Fake<ICustomerRepository>();
        _csvImporter = new(_fakeCustomerRepo);
    }

    private Stream GetStreamFromString(string content) => new MemoryStream(Encoding.UTF8.GetBytes(content));

    [Fact]
    public async Task OneCustomer()
    {
        string email = "amanchandra.in@gmail.com";
        string name = "Aman Chandra";
        string license = "Basic";
        string csv = string.Join(',', email, name, license);

        var stream = GetStreamFromString(csv);
        await _csvImporter.ReadAsync(stream);
    }

    [Fact]
    public async Task ValidCustomerOneLine()
    {
        /* Arrange */
        string email = "amanchandra.in@gmail.com";
        string name = "Aman Chandra";
        string license = "Basic";
        string csv = string.Join(',', email, name, license);
        A.CallTo(() => _fakeCustomerRepo.GetByEmailAsync(email))
            .Returns(default(Customer));

        /* Act */
        var stream = GetStreamFromString(csv);
        await _csvImporter.ReadAsync(stream);

        /* Assert */
        A.CallTo(() => _fakeCustomerRepo.GetByEmailAsync(email))
            .MustHaveHappened(); // verification

        A.CallTo(() => _fakeCustomerRepo.CreateAsync(A<NewCustomerDto>.That.Matches(n =>
            n.Email == email && n.Name == name && n.License == license
        ))).MustHaveHappened();
    }

    [Fact]
    public async Task InvalidLine()
    {
        var stream = GetStreamFromString("this is an invalid line");
        await _csvImporter.ReadAsync(stream);

        var calls = Fake.GetCalls(_fakeCustomerRepo);
        Assert.Empty(calls); // no calls are made to the fakeRepo's methods
    }

    [Fact]
    public async Task ThreeLinesOneInvalid()
    {
        /* Arrange */
        A.CallTo(() => _fakeCustomerRepo.GetByEmailAsync(A<string>.Ignored))
            .Returns(default(Customer));

        /* Act */
        var stream = GetStreamFromString("a@b.com,customer1,basic\ninvalidline\nc@d.com,customer2,premium");
        await _csvImporter.ReadAsync(stream);

        /* Assert */
        A.CallTo(() => _fakeCustomerRepo.CreateAsync(A<NewCustomerDto>.Ignored))
            .MustHaveHappenedTwiceExactly();
    }

    [Fact]
    public async Task GetThrows()
    {
        A.CallTo(() => _fakeCustomerRepo.GetByEmailAsync(""))
            .Throws<ArgumentException>();

        var stream = GetStreamFromString(",name,license");
        await Assert.ThrowsAsync<ArgumentException>(
            () => _csvImporter.ReadAsync(stream)
        );
    }

    /* Arrange, Act, Assert Pattern (AAA)
    *  
    *  Arrange - intializes the objects and sets the value of the data to be passed to the method under test
    *  Act - invokes the method under test with the arranged parameters
    *  Assert - verifies the action of the method under test, and behaves as expected.
    */
}
