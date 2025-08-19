using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore.Storage;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using TesteFacil.Infraestrutura.Orm.Compartilhado;
using static System.Net.WebRequestMethods;

namespace TesteFacil.Testes.Interface.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected static IWebDriver? driver;
    protected static TesteFacilDbContext? dbContext;

    protected static string enderecoBase = "https://localhost:7056";
    private static string connectionString = "Host=localhost;Port=5432;Database=AcademiaDoProgramadorDb;Username=postgres;Password=YourStrongPassword";

    private static IDatabaseContainer? dbContainer;
    private readonly static int dbPort = 5432;

    [AssemblyInitialize]
    public static async Task ConfigurarTestes(TestContext _)
    {
        await InicializarBancoDadosAsync();

        InicializarWebDriver();
    }

    [AssemblyCleanup]
    public static async Task EncerrarTestes()
    {
        await EncerrarBancoDadosAsync();

        EncerrarWebDriver();
    }

    [TestInitialize]
    public void InicializarTestes()
    {
        dbContext = TesteFacilDbContextFactory.CriarDbContext(connectionString);

        ConfigurarTabelas(dbContext);
    }

    private static async Task InicializarBancoDadosAsync()
    {
        dbContainer = new PostgreSqlBuilder()
       .WithImage("postgres:16")
       .WithPortBinding(dbPort, true)
       .WithName("teste-facil-testedb")
       .WithDatabase("TesteFacilTesteDb")
       .WithUsername("postgres")
       .WithPassword("YourStrongPassword")
       .WithCleanUp(true)
       .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(dbPort))
       .Build();

        await dbContainer.StartAsync();
    }

    private static void InicializarWebDriver()
    {
        var options = new FirefoxOptions();
        options.AddArgument("--headless=new");

        driver = new FirefoxDriver(options);
    }
    
    private static async Task EncerrarBancoDadosAsync()
    {
        if(dbContainer is not null)
           await dbContainer.DisposeAsync();
    }
    private static void EncerrarWebDriver()
    {
        driver?.Quit();
        driver?.Dispose();
    }

    private static void ConfigurarTabelas(TesteFacilDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.Testes.RemoveRange(dbContext.Testes);
        dbContext.Questoes.RemoveRange(dbContext.Questoes);
        dbContext.Materias.RemoveRange(dbContext.Materias);
        dbContext.Disciplinas.RemoveRange(dbContext.Disciplinas);

        dbContext.SaveChanges();
    }
}
