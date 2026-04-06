using FluentAssertions;
using MinhasFinancas.Domain.Entities;
using Xunit;
using Tipo = MinhasFinancas.Domain.Entities.Transacao.ETipo;

namespace MinhasFinancas.UnitTests.BusinessRules;

/// <summary>
/// Casos de borda para Categoria.PermiteTipo — cobre os 6 combinações de finalidade×tipo.
/// </summary>
public class CategoriaBordaTests
{
    [Theory(DisplayName = "permiteTipo_tabela_completa")]
    [InlineData(Categoria.EFinalidade.Receita, Tipo.Receita, true)]
    [InlineData(Categoria.EFinalidade.Receita, Tipo.Despesa, false)]
    [InlineData(Categoria.EFinalidade.Despesa, Tipo.Despesa, true)]
    [InlineData(Categoria.EFinalidade.Despesa, Tipo.Receita, false)]
    [InlineData(Categoria.EFinalidade.Ambas, Tipo.Receita, true)]
    [InlineData(Categoria.EFinalidade.Ambas, Tipo.Despesa, true)]
    public void permiteTipo_tabela(Categoria.EFinalidade finalidade, Tipo tipo, bool esperado)
    {
        var cat = new Categoria { Finalidade = finalidade };
        cat.PermiteTipo(tipo).Should().Be(esperado,
            $"finalidade={finalidade} + tipo={tipo} deveria retornar {esperado}");
    }
}
