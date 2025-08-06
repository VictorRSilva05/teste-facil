using TesteFacil.Dominio.ModuloDisciplina;
using TesteFacil.Dominio.ModuloMateria;
using TesteFacil.Dominio.ModuloQuestao;

namespace TesteFacil.Testes.Unidade.ModuloDisciplina;

[TestClass]
[TestCategory("Testes de Unidade de Disciplina")]
public sealed class DisciplinaTests
{
    private Disciplina? disciplina;

    [TestMethod]
    public void Deve_SortearQuestoes_DaDisciplina_Corretamente()
    {
        //Arrange 
        disciplina = new Disciplina("Matemática");

        var materiaQuatroOperacoes = new Materia(
            "Quatro Operações",
            SerieMateria.SegundaSerie,
            disciplina
            );

        var materiaFracoes = new Materia(
            "Fracoes",
            SerieMateria.SegundaSerie,
            disciplina
            );

        materiaQuatroOperacoes.AdicionarQuestoes([
            new Questao("Quanto é 2 + 2", materiaQuatroOperacoes),
            new Questao("Quanto é 3 + 5", materiaQuatroOperacoes),
            new Questao("Quanto é 10 - 4", materiaQuatroOperacoes),
            new Questao("Quanto é 6 * 7", materiaQuatroOperacoes),
            new Questao("Quanto é 8 / 2", materiaQuatroOperacoes)
            ]);

        materiaFracoes.AdicionarQuestoes([
            new Questao("Quanto é 1/2 + 1/2", materiaFracoes),
            new Questao("Quanto é 3/4 - 1/4", materiaFracoes),
            new Questao("Quanto é 2/3 * 3/4", materiaFracoes),
            new Questao("Quanto é 5/6 / 1/2", materiaFracoes),
            new Questao("Quanto é 3/4 de 100", materiaFracoes)
            ]);

        disciplina.AdicionarMateria(materiaQuatroOperacoes);
        disciplina.AdicionarMateria(materiaFracoes);

        //Act
        var questoesSorteadas = disciplina.ObterQuestoesAleatorias(10, SerieMateria.SegundaSerie);

        //Assert
        List<Questao> questoesEsperadas = [..materiaQuatroOperacoes.Questoes,  ..materiaFracoes.Questoes];

        Assert.AreEqual(10, questoesSorteadas.Count);

        CollectionAssert.IsSubsetOf(questoesSorteadas, questoesEsperadas);
    }

    [TestMethod]
    public void Deve_AdicionarMateria_ADisciplina_Corretamente()
    {
        //Arrange
        disciplina = new Disciplina("Matemática");

        var materia = new Materia("Quatro Operações", SerieMateria.PrimeiraSerie,disciplina);

        //Act
        disciplina.AdicionarMateria(materia);

        //Assert
        var disciplinaContemMateria = disciplina.Materias.Contains(materia);

        Assert.IsTrue(disciplinaContemMateria);
    }
}
