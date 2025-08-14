using OpenQA.Selenium;
using TesteFacil.Testes.Interface.Compartilhado;

namespace TesteFacil.Testes.Interface.ModuloDisciplina;

[TestClass]
[TestCategory("Tests de interface de Disciplina")]
public sealed class DisciplinaInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Disciplina_Corretamente()
    {
        //Arrange
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "disciplinas"));

        var elemento = driver.FindElement(By.CssSelector("a[data-se='btnCadastrar']"));

        elemento?.Click();

        //Act
        driver?.FindElement(By.Id("Nome")).SendKeys("Matemática");

        driver?.FindElement(By.CssSelector("button[type='submit']")).Click();

        //Assert
        var elementosCard = driver?.FindElements(By.CssSelector(".card"));

        Assert.AreEqual(1, elementosCard.Count());
    }

    [TestMethod]
    public void Deve_Editar_Disciplina()
    {
        //Arrange
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "disciplinas"));

        var elemento = driver.FindElement(By.CssSelector("a[data-se='btnCadastrar']"));

        elemento?.Click();

        driver?.FindElement(By.Id("Nome")).SendKeys("Matemática");

        driver?.FindElement(By.CssSelector("button[type='submit']")).Click();

        driver?.FindElement(By.CssSelector(".card"))
            .FindElement(By.CssSelector("a[title='Edição']")).Click();

        //Act
        driver?.FindElement(By.Id("Nome")).SendKeys(" Editada");

        driver?.FindElement(By.CssSelector("button[type='submit']")).Click();

        //Assert
        Assert.IsTrue(driver?.PageSource.Contains("Matemática Editada"));
    }
}
