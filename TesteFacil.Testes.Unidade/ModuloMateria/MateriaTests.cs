using TesteFacil.Dominio.ModuloDisciplina;
using TesteFacil.Dominio.ModuloMateria;
using TesteFacil.Dominio.ModuloQuestao;

namespace TesteFacil.Testes.Unidade.ModuloMateria;

[TestClass]
[TestCategory("Testes de Unidade de Matéria")]
public sealed class MateriaTests
{
    private Materia? materia;

    [TestMethod]
    public void Deve_AdicionarQuestao_AMateria_Corretamente()
    {
        //Arrange
        var disciplina = new Disciplina("Matemática");

        materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        Questao questao = new Questao("Quanto é 2+2?", materia);

        //Act
        materia.AdicionarQuestao(questao);

        //Assert
        var materiaContemQuestao = materia.Questoes.Contains(questao);

        Assert.IsTrue(materiaContemQuestao);
    }

    [TestMethod]
    public void Deve_RemoverQuestao_DeMateria_Corretamente()
    {
        //Arrange
        var disciplina = new Disciplina("Matemática");

        materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        Questao questao = new Questao("Quanto é 2+2?", materia);

        //Act
        materia.RemoverQuestao(questao);

        //Assert
        var materiaNaoContemQuestao = materia.Questoes.Contains(questao);

        Assert.IsFalse(materiaNaoContemQuestao);
    }

    [TestMethod]
    public void Deve_ObterQuestoes_Aleatoriamente_Corretamente()
    {
        //Arrange
        var disciplina = new Disciplina("Matemática");

        materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        List<Questao> questoesAdicionadas = new()
        {
            new Questao("Quanto é 2+2?", materia),
            new Questao("Quanto é 3+5?", materia),
            new Questao("Quanto é 10-4?", materia),
            new Questao("Quanto é 6*7?", materia),
            new Questao("Quanto é 8/2?", materia)
        };

        foreach (var questao in questoesAdicionadas)
            materia.AdicionarQuestao(questao);

        //Act
        var questoesSorteadas = materia.ObterQuestoesAleatorias(3);

        //Assert
        Assert.AreEqual(3, questoesSorteadas.Count);
        CollectionAssert.IsSubsetOf(questoesSorteadas, questoesAdicionadas);
    }

    [TestMethod]
    public void Deve_AtualizarRegistro_Corretamente()
    {
        //Arrange
        var disciplina = new Disciplina("Matemática");

        materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        var materiaEditada = new Materia("Frações", SerieMateria.SegundaSerie, disciplina);

        //Act

        materia.AtualizarRegistro(materiaEditada);

        //Assert
        Assert.AreEqual(materiaEditada.Nome, materia.Nome);
        Assert.AreEqual(materiaEditada.Serie, materia.Serie);
        Assert.AreEqual(materiaEditada.Disciplina, materia.Disciplina);
    }
}
