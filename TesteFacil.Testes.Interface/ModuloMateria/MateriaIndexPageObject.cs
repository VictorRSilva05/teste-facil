using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TesteFacil.Testes.Interface.ModuloDisciplina;

namespace TesteFacil.Testes.Interface.ModuloMateria;

public class MateriaFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public MateriaFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);
    }

    public MateriaFormPageObject PreencherNome(string nome)
    {
        wait.Until(d =>
            d.FindElement(By.Id("Nome")).Displayed &&
            d.FindElement(By.Id("Nome")).Enabled
        );

        var inputNome = driver.FindElement(By.Id("Nome"));
        inputNome.Clear();
        inputNome.SendKeys(nome);

        return this;
    }

    public MateriaFormPageObject SelecionarSerie(string serie)
    {
        wait.Until(d =>
            d.FindElement(By.Id("Serie")).Displayed &&
            d.FindElement(By.Id("Serie")).Enabled
        );

        var select = new SelectElement(driver.FindElement(By.Id("Serie")));
        select.SelectByText(serie);

        return this;
    }

    public MateriaFormPageObject SelecionarDisciplina(string disciplina)
    {
        wait.Until(d =>
            d.FindElement(By.Id("DisciplinaId")).Displayed &&
            d.FindElement(By.Id("DisciplinaId")).Enabled
        );

        var select = new SelectElement(driver.FindElement(By.Id("DisciplinaId")));
        select.SelectByText(disciplina);

        return this;
    }

    public MateriaIndexPageObject Confirmar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']"))).Click();

        return new MateriaIndexPageObject(driver);
    }
}
public class MateriaIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public MateriaIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public MateriaIndexPageObject IrPara(string enderecoBase)
    {
        driver?.Navigate().GoToUrl(Path.Combine(enderecoBase, "materias"));

        return this;
    }

    public MateriaFormPageObject ClickCadastrar()
    {
        driver?.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Click();

        return new MateriaFormPageObject(driver!);
    }

    public MateriaFormPageObject ClickEditar()
    {
        driver?.FindElement(By.CssSelector(".card a[title='Edição']")).Click();

        return new MateriaFormPageObject(driver!);
    }

    public MateriaFormPageObject ClickExcluir()
    {
        driver?.FindElement(By.CssSelector(".card a[title='Exclusão']")).Click();

        return new MateriaFormPageObject(driver!);
    }

    public bool ContemMateria(string nome)
    {
        return driver.PageSource.Contains(nome);
    }
}