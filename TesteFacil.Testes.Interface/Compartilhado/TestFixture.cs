using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
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

    [AssemblyInitialize]
    public static void ConfigurarTestes(TestContext _)
    {
        InicializarWebDriver();
    }

    [AssemblyCleanup]
    public static void EncerrarTestes()
    {
        EncerrarWebDriver();
    }

    [TestInitialize]
    public void InicializarTestes()
    {
        dbContext = TesteFacilDbContextFactory.CriarDbContext(connectionString);

        ConfigurarTabelas(dbContext);
    }


    private static void InicializarWebDriver()
    {
        var options = new FirefoxOptions();
        options.AddArgument("--headless=new");

        driver = new FirefoxDriver(options);
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
