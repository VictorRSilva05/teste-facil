using TesteFacil.Dominio.ModuloDisciplina;
using TesteFacil.Dominio.ModuloMateria;
using TesteFacil.Dominio.ModuloQuestao;
using TesteFacil.Dominio.ModuloTeste;

namespace TesteFacil.Testes.Unidade.ModuloTeste;

[TestClass]
[TestCategory("Testes de Unidade de Testes")]
public sealed class TesteTests
{
    [TestMethod]
    public void DeveAdicionar_QuestaoATeste_Corretamente()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");

        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        Questao questao = new Questao("Quanto é 2+2?", materia);

        Teste teste = new Teste("Prova de matemática", false, 1, SerieMateria.PrimeiraSerie, disciplina, materia);

        //Act
        teste.AdicionarQuestao(questao);

        //Assert
        Assert.IsTrue(teste.Questoes.Contains(questao));
    }

    [TestMethod]
    public void DeveRemover_QuestaoDoTeste_Corretamente()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");

        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        Questao questao = new Questao("Quanto é 2+2?", materia);

        Teste teste = new Teste("Prova de matemática", false, 1, SerieMateria.PrimeiraSerie, disciplina, materia);

        teste.AdicionarQuestao(questao);

        //Act
        teste.RemoverQuestao(questao);

        //Assert
        bool questaoContemAlternativa = teste.Questoes.Contains(questao);
        Assert.IsFalse(questaoContemAlternativa);
    }

    [TestMethod]
    public void DeveRemover_TodasAsQuestoes_DoTeste_Corretamente()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");

        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        List<Questao> questoes = new List<Questao>
        {
            new Questao("Quanto é 2+2?", materia),
            new Questao("Quanto é 2-3?", materia),
            new Questao("Quanto é 7+9?", materia)
        };

        Teste teste = new Teste("Prova de matemática", false, 1, SerieMateria.PrimeiraSerie, disciplina, materia, questoes);

        //Act
        teste.RemoverQuestoesAtuais();

        //Assert
        bool testeNaoContemQuestoes = teste.Questoes.Count() == 0;
        Assert.IsTrue(testeNaoContemQuestoes);
    }

    [TestMethod]
    public void Deve_AtualizarORegistro_Corretamente()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);
        List<Questao> questoes = new List<Questao>
        {
            new Questao("Quanto é 2+2?", materia),
            new Questao("Quanto é 2-3?", materia),
            new Questao("Quanto é 7+9?", materia)
        };

        Teste teste = new Teste("Prova de matemática", false, 1, SerieMateria.PrimeiraSerie, disciplina, materia, questoes);

        Disciplina disciplina1 = new Disciplina("Física");
        Materia materia1 = new Materia("Estados da matéria", SerieMateria.SegundaSerie, disciplina1);
        List<Questao> questoes1 = new List<Questao>
        {
            new Questao("Qual o estado natural da água", materia1),
            new Questao("Qual o estado de Pernambuco?", materia1)
        };

        Teste testeEditado = new Teste("Prova de física", false, 2, SerieMateria.SegundaSerie, disciplina1, materia1, questoes1);

        //Act
        teste.AtualizarRegistro(testeEditado);

        //Assert
        Assert.AreEqual(teste.Titulo, testeEditado.Titulo);
        Assert.AreEqual(teste.Serie, testeEditado.Serie);
        Assert.AreEqual(teste.Recuperacao, testeEditado.Recuperacao);
        Assert.AreEqual(teste.QuantidadeQuestoes, teste.QuantidadeQuestoes);
        Assert.AreEqual(teste.Questoes, testeEditado.Questoes);
    }

    [TestMethod]
    public void DeveSortear_QuestoesDoTeste_Corretamente()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);
        List<Questao> questoesAdicionadas = new List<Questao>
        {
            new Questao("Quanto é 2+2?", materia),
            new Questao("Quanto é 3+5?", materia),
            new Questao("Quanto é 10-4?", materia),
            new Questao("Quanto é 6*7?", materia),
            new Questao("Quanto é 8/2?", materia)
        };
        foreach (var questao in questoesAdicionadas)
            materia.AdicionarQuestao(questao);
        Teste teste = new Teste("Prova de matemática", false, 3, SerieMateria.PrimeiraSerie, disciplina, materia);
        //Act
        var questoesSorteadas = teste.SortearQuestoes();
        //Assert
        Assert.AreEqual(3, questoesSorteadas.Count);
        CollectionAssert.IsSubsetOf(questoesSorteadas, questoesAdicionadas);
    }
}
