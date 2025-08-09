using TesteFacil.Dominio.ModuloDisciplina;
using TesteFacil.Dominio.ModuloMateria;
using TesteFacil.Dominio.ModuloQuestao;

namespace TesteFacil.Testes.Unidade.ModuloQuestao;

[TestClass]
[TestCategory("Testes de Unidade de Questoes")]
public sealed class QuestaoTests
{
    private Questao? questao;

    [TestMethod]
    public void DeveAdicionar_AlternativaAQuestao_Corretamente()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");

        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        Questao questao = new Questao("Quanto é 2+2?", materia);

        //Act
        Alternativa alternativa = questao.AdicionarAlternativa("4", true);

        //Assert
        var questaoContemAlternativa = questao.Alternativas.Contains(alternativa);
        Assert.IsTrue(questaoContemAlternativa);
    }

    [TestMethod]
    public void DeveRemover_AlternativaDaQuestao_Corretamente()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");

        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        Questao questao = new Questao("Quanto é 2+2?", materia);

        Alternativa alternativa = questao.AdicionarAlternativa("4", true);

        //Act
        questao.RemoverAlternativa('a');

        //Assert
        var questaoContemAlternativa = questao.Alternativas.Contains(alternativa);
        Assert.IsFalse(questaoContemAlternativa);
    }

    [TestMethod]
    public void DeveReatribuirLetras_AposRemoverAlternativa_Corretamente()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");

        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        Questao questao = new Questao("Quanto é 2+2?", materia);

        questao.AdicionarAlternativa("4", true);
        questao.AdicionarAlternativa("5", false);
        questao.AdicionarAlternativa("6", false);

        //Act
        questao.RemoverAlternativa('b');

        //Assert
        Assert.AreEqual('a', questao.Alternativas[0].Letra);
        Assert.AreEqual('b', questao.Alternativas[1].Letra);
    }

    [TestMethod]
    public void Deve_Atualizar_Registro_Corretamente()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);
        Questao questao = new Questao("Quanto é 2+2?", materia);

        Materia materia1 = new Materia("Física quântica", SerieMateria.SegundaSerie, disciplina);
        Questao questaoEditada = new Questao("Quanto é 60 + 7?", materia1);

        //Act
        questao.AtualizarRegistro(questaoEditada);

        //Assert
        Assert.AreEqual(questao.Enunciado, questaoEditada.Enunciado);
    }
}
