using Microsoft.Extensions.Logging;
using TesteFacil.Aplicacao.ModuloMateria;
using TesteFacil.Dominio.Compartilhado;
using TesteFacil.Dominio.ModuloDisciplina;
using TesteFacil.Dominio.ModuloMateria;
using TesteFacil.Dominio.ModuloQuestao;
using TesteFacil.Dominio.ModuloTeste;
using Moq;

namespace TesteFacil.Testes.Unidade.ModuloMateria;

[TestClass]
[TestCategory("Testes de Unidade de Matéria")]
public sealed class MateriaAppServiceTests
{
    private Mock<IRepositorioMateria> repositorioMateriaMock;
    private Mock<IRepositorioDisciplina> repositorioDisciplinaMock;
    private Mock<IRepositorioQuestao> repositorioQuestaoMock;
    private Mock<IRepositorioTeste> repositorioTesteMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<MateriaAppService>> loggerMock;

    private MateriaAppService materiaAppService;

    [TestInitialize]
    public void Setup()
    {
        repositorioMateriaMock = new Mock<IRepositorioMateria>();
        repositorioDisciplinaMock = new Mock<IRepositorioDisciplina>();
        repositorioQuestaoMock = new Mock<IRepositorioQuestao>();
        repositorioTesteMock = new Mock<IRepositorioTeste>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<MateriaAppService>>();

        materiaAppService = new MateriaAppService(
            repositorioDisciplinaMock.Object,
            repositorioMateriaMock.Object,
            repositorioQuestaoMock.Object,
            repositorioTesteMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    [TestMethod]
    public void Cadastrar_deve_retornar_ok_quando_materia_for_valida()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        repositorioMateriaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Materia>());

        //Act
        var resultado = materiaAppService.Cadastrar(materia);

        //Assert
        repositorioMateriaMock.Verify(r => r.Cadastrar(materia), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_deve_retornar_falha_quando_a_materia_for_duplicada()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);
        
        Materia materiaTeste = new Materia("Quatro operações", SerieMateria.SegundaSerie, disciplina);

        repositorioMateriaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Materia>() { materiaTeste});

        //Act
        var resultado = materiaAppService.Cadastrar(materia);

        //Assert
        repositorioMateriaMock?.Verify(r => r.Cadastrar(materia),Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Cadastrar_deve_retornar_falha_quando_excecao_for_lancada()
    {
        //Arrange
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Quatro operações", SerieMateria.PrimeiraSerie, disciplina);

        repositorioMateriaMock?
              .Setup(r => r.SelecionarRegistros())
              .Returns(new List<Materia>());

        unitOfWorkMock?
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro inesperado"));            
            
        //Act
        var resultado = materiaAppService?.Cadastrar(materia);

        //Assert
        unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.AreEqual("Ocorreu um erro interno do servidor", resultado.Errors.First().Message);
        Assert.IsTrue(resultado.IsFailed);
    }
}
