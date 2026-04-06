using FluentAssertions;
using MinhasFinancas.Domain.Entities;
using Xunit;
using Tipo = MinhasFinancas.Domain.Entities.Transacao.ETipo;

namespace MinhasFinancas.UnitTests.BusinessRules;

public class CategoriaPermiteTipoTests
{
    [Fact(DisplayName = "should_allow_receita_when_categoria_is_receita")]
    public void should_allow_receita_when_categoria_is_receita()
    {
        var cat = new Categoria { Finalidade = Categoria.EFinalidade.Receita };
        cat.PermiteTipo(Tipo.Receita).Should().BeTrue();
    }

    [Fact(DisplayName = "should_not_allow_receita_when_categoria_is_somente_despesa")]
    public void should_not_allow_receita_when_categoria_is_somente_despesa()
    {
        var cat = new Categoria { Finalidade = Categoria.EFinalidade.Despesa };
        cat.PermiteTipo(Tipo.Receita).Should().BeFalse();
    }

    [Fact(DisplayName = "should_not_allow_despesa_when_categoria_is_somente_receita")]
    public void should_not_allow_despesa_when_categoria_is_somente_receita()
    {
        var cat = new Categoria { Finalidade = Categoria.EFinalidade.Receita };
        cat.PermiteTipo(Tipo.Despesa).Should().BeFalse();
    }

    [Fact(DisplayName = "should_allow_both_tipos_when_categoria_is_ambas")]
    public void should_allow_both_tipos_when_categoria_is_ambas()
    {
        var cat = new Categoria { Finalidade = Categoria.EFinalidade.Ambas };
        cat.PermiteTipo(Tipo.Receita).Should().BeTrue();
        cat.PermiteTipo(Tipo.Despesa).Should().BeTrue();
    }
}
