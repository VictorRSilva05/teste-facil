using FizzWare.NBuilder;
using TesteFacil.Dominio.ModuloDisciplina;
using TesteFacil.Dominio.ModuloMateria;
using TesteFacil.Dominio.ModuloQuestao;
using TesteFacil.Testes.Integracao.Compartilhado;

namespace TesteFacil.Testes.Integracao.ModuloQuestao;

[TestClass]
[TestCategory("Teste de Integração de Questão")]
public sealed class RepositorioQuestaoEmOrm : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Questao_Corretamente()
    {
        //Arrange
        var disciplina = Builder<Disciplina>.CreateNew()
            .With(d => d.Nome = "Matemática")
            .Persist();

        var materia = Builder<Materia>.CreateNew()
            .With(m => m.Nome = "Quatro Operações")
            .With(m => m.Disciplina = disciplina)
            .Persist();

        var questao = new Questao("Quanto é 2 + 2?", materia);

        //Act
        repositorioQuestao?.Cadastrar(questao);
        dbContext?.SaveChanges();

        //Assert
        var registroSelecionado = repositorioQuestao?.SelecionarRegistroPorId(questao.Id);

        Assert.AreEqual(questao, registroSelecionado);
    }
}
