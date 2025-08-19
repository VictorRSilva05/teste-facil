using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
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

    private static IContainer? appContainer;
    private readonly static int appPort = 8080;

    private static IConfiguration? configuracao;

    [AssemblyInitialize]
    public static async Task ConfigurarTestes(TestContext _)
    {
        configuracao = new ConfigurationBuilder()
            .AddUserSecrets<TestFixture>()
            .AddEnvironmentVariables()
            .Build();

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

    private static async Task InicializarAplicacaoAsync()
    {
        var imagem = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithDockerfile("Dockerfile")
            .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
            .WithName("teste-facil-app-e2e:latest")
            .Build();   

        await imagem.CreateAsync().ConfigureAwait(false);

        appContainer = new ContainerBuilder()
            .WithImage(imagem)
            .WithPortBinding(appPort, true)
            .WithName("teste-facil-webapp")
            .WithEnvironment("SQL_CONNECTION_STRING", configuracao["SQL_CONNECTION_STRING"])
            .WithEnvironment("GEMINI_API_KEY", configuracao["GEMINI_API_KEY"])
            .WithEnvironment("NEWRELIC_LICENSE_KEY", configuracao["NEWRELIC_LICENSE_KEY"])
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(appPort)
                .UntilHttpRequestIsSucceeded(r => r.ForPort((ushort)appPort).ForPath("/health"))
                )
            .WithCleanUp(true)
            .Build();
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
