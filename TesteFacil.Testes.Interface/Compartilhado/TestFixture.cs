using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
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

    protected static string enderecoBase;
    private static string connectionString = "Host=localhost;Port=5432;Database=AcademiaDoProgramadorDb;Username=postgres;Password=YourStrongPassword";

    private static IDatabaseContainer? dbContainer;
    private readonly static int dbPort = 5432;

    private static IContainer? appContainer;
    private readonly static int appPort = 8080;

    private static IContainer? seleniumContainer;
    private readonly static int seleniumPort = 4444;

    private static IConfiguration? configuracao;

    [AssemblyInitialize]
    public static async Task ConfigurarTestes(TestContext _)
    {
        configuracao = new ConfigurationBuilder()
            .AddUserSecrets<TestFixture>()
            .AddEnvironmentVariables()
            .Build();

        await InicializarBancoDadosAsync();

        await InicializarAplicacaoAsync();

        InicializarWebDriverAsync();
    }

    [AssemblyCleanup]
    public static async Task EncerrarTestes()
    {
        EncerrarWebDriverAsync();

        await EncerrarBancoDadosAsync();

        await EncerrarAplicacaoAsync();
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

        await appContainer.StartAsync();

        enderecoBase = $"http://{appContainer.Name}:{appPort}";
    }
    private static async Task InicializarWebDriverAsync()
    {
        seleniumContainer = new ContainerBuilder()
            .WithImage("selenium/standalone-chrome:nightly")
            .WithPortBinding(seleniumPort, true)
            .WithName("teste-facil-selenium-e2e")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(seleniumPort)
            )
            .Build();

        await seleniumContainer.StartAsync();

        var enderecoSelenium = new Uri($"http://{seleniumContainer.Hostname}:{seleniumContainer.GetMappedPublicPort(seleniumPort)}/wd/hub");
   
        var options = new FirefoxOptions();
        options.AddArgument("--headless=new");

        driver = new RemoteWebDriver(enderecoSelenium,options);
    }
    
    private static async Task EncerrarBancoDadosAsync()
    {
        if(dbContainer is not null)
           await dbContainer.DisposeAsync();
    }

    private static async Task EncerrarAplicacaoAsync()
    {
        if (appContainer is not null)
            await appContainer.DisposeAsync();
    }

    private static async Task EncerrarWebDriverAsync()
    {
        driver?.Quit();
        driver?.Dispose();

        if (seleniumContainer is not null)
            await seleniumContainer.DisposeAsync();
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
