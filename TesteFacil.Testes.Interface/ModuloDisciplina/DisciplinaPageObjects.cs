using DotNet.Testcontainers.Builders;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TesteFacil.Testes.Interface.ModuloDisciplina;

public class DisciplinaFormPageObject
{
 private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public DisciplinaFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);
    }

    public DisciplinaFormPageObject PreencherNome(string nome)
    {
        var inputNome = driver?.FindElement(By.Id("Nome"));
        inputNome?.Clear();
        inputNome?.SendKeys(nome);

        return this;
    }

    public DisciplinaIndexPageObject Confirmar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']"))).Click();

        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return new DisciplinaIndexPageObject(driver!);
    }
}

public class DisciplinaIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public DisciplinaIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public DisciplinaIndexPageObject IrPara(string enderecoBase)
    {
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "disciplinas"));

        return this;
    }
    public DisciplinaFormPageObject ClickCadastrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']"))).Click();

        return new DisciplinaFormPageObject(driver!);
    }

    public DisciplinaFormPageObject ClickEditar()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[title='Edição']"))).Click();

        return new DisciplinaFormPageObject(driver!);
    }

    public DisciplinaFormPageObject ClickExcluir()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[title='Exclusão']"))).Click();

        return new DisciplinaFormPageObject(driver!);
    }

    public bool ContemDisciplina(string nome)
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return driver.PageSource.Contains(nome);
    }
}
