using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TesteFacil.Testes.Interface.Compartilhado;

namespace TesteFacil.Testes.Interface.ModuloDisciplina;

[TestClass]
[TestCategory("Tests de interface de Disciplina")]
public sealed class DisciplinaInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Disciplina_Corretamente()
    {
        // Arrange
        var disciplinaIndex = new DisciplinaIndexPageObject(driver!);

        disciplinaIndex
            .IrPara(enderecoBase);

        // Act
        disciplinaIndex
            .ClickCadastrar()
            .PreencherNome("Matemática")
            .Confirmar();

        // Assert
        Assert.IsTrue(disciplinaIndex.ContemDisciplina("Matemática"));
    }

    [TestMethod]
    public void Deve_Editar_Disciplina_Corretamente()
    {
        // Arrange
        var indexPageObject = new DisciplinaIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        indexPageObject
            .ClickCadastrar()
            .PreencherNome("Matemática")
            .Confirmar();

        // Act
        indexPageObject
            .ClickEditar()
            .PreencherNome("Matemática Editada")
            .Confirmar();

        // Assert
        Assert.IsTrue(indexPageObject.ContemDisciplina("Matemática Editada"));
    }

    [TestMethod]
    public void Deve_Excluir_Disciplina_Corretamente()
    {
        // Arrange
        var disciplinaIndex = new DisciplinaIndexPageObject(driver!);

        disciplinaIndex
           .IrPara(enderecoBase)
           .ClickCadastrar()
           .PreencherNome("Matemática")
           .Confirmar();

        // Act
        disciplinaIndex
           .ClickExcluir()
           .Confirmar();

        // Assert
        Assert.IsFalse(disciplinaIndex.ContemDisciplina("Matemática"));
    }
}
