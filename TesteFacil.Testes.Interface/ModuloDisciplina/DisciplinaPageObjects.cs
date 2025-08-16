using OpenQA.Selenium;

namespace TesteFacil.Testes.Interface.ModuloDisciplina;

public class DisciplinaFormPageObject
{
    private readonly IWebDriver driver;

    public DisciplinaFormPageObject(IWebDriver driver)
    {
        this.driver = driver;
    }

    public DisciplinaFormPageObject PreencherNome(string nome)
    {
        var inputNome = driver?.FindElement(By.Id("Nome"));
        inputNome.Clear();
        inputNome.SendKeys("Matemática");

        return this;
    }

    public DisciplinaIndexPageObject Confirmar()
    {
        driver?.FindElement(By.CssSelector("button[type='submit']")).Click();

        return new DisciplinaIndexPageObject(driver);
    }
}

public class DisciplinaIndexPageObject
{
    private readonly IWebDriver driver;

    public DisciplinaIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;
    }

    public DisciplinaIndexPageObject IrPara(string enderecoBase)
    {
        driver?.Navigate().GoToUrl(Path.Combine(enderecoBase, "disciplinas"));

        return this;
    }

    public DisciplinaFormPageObject ClickCadastrar()
    {
        driver?.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Click();

        return new DisciplinaFormPageObject(driver!);
    }

    public bool ContemDisciplina(string nome)
    {
        return driver.PageSource.Contains(nome);
    }
}
