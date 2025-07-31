using FluentResults;
using Microsoft.Extensions.Logging;
using TesteFacil.Aplicacao.Compartilhado;
using TesteFacil.Dominio.Compartilhado;
using TesteFacil.Dominio.ModuloDisciplina;
using TesteFacil.Dominio.ModuloMateria;
using TesteFacil.Dominio.ModuloTeste;

namespace TesteFacil.Aplicacao.ModuloDisciplina;

public class DisciplinaAppService
{
    private readonly IRepositorioDisciplina repositorioDisciplina;
    private readonly IRepositorioMateria repositorioMateria;
    private readonly IRepositorioTeste repositorioTeste;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<DisciplinaAppService> logger;

    public DisciplinaAppService(
        IRepositorioDisciplina repositorioDisciplina,
        IRepositorioMateria repositorioMateria,
        IRepositorioTeste repositorioTeste,
        IUnitOfWork unitOfWork, 
        ILogger<DisciplinaAppService> logger
        )
    {
        this.repositorioDisciplina = repositorioDisciplina;
        this.repositorioMateria = repositorioMateria;
        this.repositorioTeste = repositorioTeste;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public Result<string> Cadastrar(Disciplina disciplina)
    {
        var registros = repositorioDisciplina.SelecionarRegistros();

        if (registros.Any(i => i.Nome.Equals(disciplina.Nome)))
        {
            return Result.Fail("Já existe uma disciplina registrada com este nome.");
        }

        try
        {
            repositorioDisciplina.Cadastrar(disciplina);

            unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante o registro de {@ViewModel}.",
                disciplina
            );
        }

        return Result.Ok();
    }

    public Result<Disciplina> SelecionarPorId(Guid id)
    {
        var registroSelecionado = repositorioDisciplina.SelecionarRegistroPorId(id);

        if (registroSelecionado is null)
            return Result.Fail("Não foi possível encontrar o registro.");

        return registroSelecionado;
    }

    public Result Editar(Guid id, Disciplina disciplinaEditada)
    {
        var registros = repositorioDisciplina.SelecionarRegistros();

        if (registros.Any(i => !i.Id.Equals(id) && i.Nome.Equals(disciplinaEditada.Nome)))
            return Result.Fail("Já existe uma disciplina registrada com este nome.");

        try
        {
            repositorioDisciplina.Editar(id, disciplinaEditada);

            unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a edição do registro {@ViewModel}.",
                disciplinaEditada
            );
        }

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        try
        {
            var materias = repositorioMateria.SelecionarRegistros();

            if (materias.Any(m => m.Disciplina.Id.Equals(id)))
            {
                var erro = ResultadosErro
                    .ExclusaoBloqueadaErro("A disciplina não pôde ser excluída pois está em uma ou mais matérias ativas.");

                return Result.Fail(erro);
            }

            var testes = repositorioTeste.SelecionarRegistros();

            if (testes.Any(t => t.Disciplina.Id.Equals(id)))
            {
                var erro = ResultadosErro
                    .ExclusaoBloqueadaErro("A disciplina não pôde ser excluída pois está em um ou mais testes ativos.");

                return Result.Fail(erro);
            }

            repositorioDisciplina.Excluir(id);

            unitOfWork.Commit();

            return Result.Ok();

        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a exclusão do registro {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<Disciplina>> SelecionarTodos()
    {
        var registros = new List<Disciplina>();

        try
        {
            registros = repositorioDisciplina.SelecionarRegistros();

            return Result.Ok(registros);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção de registros."
                );

            Result.Fail("Ocorreu um erro inesperado ao tentar obter os registros.");

            throw;
        }
    }
}
