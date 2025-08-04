using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using TesteFacil.Infraestrutura.Orm.Compartilhado;
using TesteFacil.Infraestrutura.Orm.ModuloDisciplina;

namespace TesteFacil.Testes.Integracao.ModuloDisciplina;

[TestClass]
[TestCategory("Testes de integração de Disciplina")]
public sealed class RepositoritorioDisciplinaEmOrmTests
{
    private TesteFacilDbContext dbContext;
    private RepositorioDisciplinaEmOrm repositorioDisciplina;

    [TestMethod]
    public void Deve_Cadastrar_Registro_Corretamente()
    {
        var configuracao = new ConfigurationBuilder().Build();
        
        
        var options = new DbContextOptionsBuilder<TesteFacilDbContext>().Options;

        dbContext = new TesteFacilDbContext(options);

        repositorioDisciplina = new RepositorioDisciplinaEmOrm(dbContext);
    }
}
