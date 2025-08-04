using FizzWare.NBuilder;
using TesteFacil.Dominio.ModuloDisciplina;
using TesteFacil.Dominio.ModuloMateria;
using TesteFacil.Infraestrutura.Orm.Compartilhado;
using TesteFacil.Infraestrutura.Orm.ModuloDisciplina;
using TesteFacil.Infraestrutura.Orm.ModuloMateria;
using TesteFacil.Testes.Integracao.Compartilhado;

namespace TesteFacil.Testes.Integracao.ModuloMateria;

[TestClass]
[TestCategory("Testes de integração de Matéria")]
public class RepositorioMateriaEmOrmTests
{
    private TesteFacilDbContext dbContext;
    private RepositorioDisciplinaEmOrm repositorioDisciplina;
    private RepositorioMateriaEmOrm repositorioMateria;

    [TestInitialize]
    public void ConfigurarTestes()
    {
        dbContext = TesteFacilDbContextFactory.CriarDbContext();

        repositorioDisciplina = new RepositorioDisciplinaEmOrm(dbContext);
        repositorioMateria = new RepositorioMateriaEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Disciplina>(repositorioDisciplina.Cadastrar);
    }

    [TestMethod]
    public void Deve_Cadastrar_Materia_Corretamente()
    {
        //Arrange
        var disciplina = Builder<Disciplina>.CreateNew().Persist();

        repositorioDisciplina.Cadastrar(disciplina);

        var materia = new Materia("Quatro operações", SerieMateria.SegundaSerie, disciplina);

        //Act
        repositorioMateria.Cadastrar(materia);
        dbContext.SaveChanges();

        //Assert
        var materiaSelecionada = repositorioMateria.SelecionarRegistroPorId(materia.Id);

        Assert.AreEqual(materia, materiaSelecionada);
    }

    [TestMethod]
    public void Deve_Selecionar_Materias_Corretamente()
    {
        //Arrange
        var disciplina = Builder<Disciplina>.CreateNew().Persist();

        var materia1 = new Materia("Quatro Operações", SerieMateria.PrimeiraSerie, disciplina);
        var materia2 = new Materia("Álgebra Linear", SerieMateria.PrimeiraSerie, disciplina);
        var materia3 = new Materia("Cálculo Númerico", SerieMateria.PrimeiraSerie, disciplina);

        repositorioMateria.Cadastrar(materia1);
        repositorioMateria.Cadastrar(materia2);
        repositorioMateria.Cadastrar(materia3);

        dbContext.SaveChanges();

        List<Materia> materiasEsperadas = [materia1, materia2, materia3];

        var materiasEsperadasOrdenadas = materiasEsperadas
            .OrderBy(d => d.Nome)
            .ToList();

        //Act
        var materiasRecebidas = repositorioMateria
            .SelecionarRegistros();

        //Assert
        CollectionAssert.AreEqual(materiasEsperadasOrdenadas, materiasRecebidas);
    }


    [TestMethod]
    public void Deve_Editar_Materia_Corretamente()
    {
        // Arrange
        var disciplina = Builder<Disciplina>.CreateNew().Persist();

        var materia1 = new Materia("Quatro Operações", SerieMateria.PrimeiraSerie, disciplina);
        repositorioMateria.Cadastrar(materia1);

        dbContext.SaveChanges();

        var materiaEditada = new Materia("Álgebra Linear", SerieMateria.SegundaSerie, disciplina);

        // Act
        var conseguiuEditar = repositorioMateria.Editar(materia1.Id, materiaEditada);
        dbContext.SaveChanges();

        // Assert
        var registroSelecionado = repositorioMateria.SelecionarRegistroPorId(materia1.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(materia1, registroSelecionado);
    }

    [TestMethod]
    public void Deve_Excluir_Materia_Corretamente()
    {
        // Arrange
        var disciplina = Builder<Disciplina>.CreateNew().Persist();
        var materia1 = new Materia("Quatro Operações", SerieMateria.PrimeiraSerie, disciplina);
        repositorioMateria.Cadastrar(materia1);

        dbContext.SaveChanges();

        // Act
        var conseguiuExcluir = repositorioMateria.Excluir(materia1.Id);
        dbContext.SaveChanges();

        // Assert
        var registroSelecionado = repositorioMateria.SelecionarRegistroPorId(disciplina.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(registroSelecionado);
    }

}
