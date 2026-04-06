using FluentAssertions;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.UnitTests.Support;
using Xunit;
using Tipo = MinhasFinancas.Domain.Entities.Transacao.ETipo;

namespace MinhasFinancas.UnitTests.BusinessRules;

public class TransacaoRegrasTests
{
    [Fact(DisplayName = "should_not_allow_income_for_underage_person")]
    public void should_not_allow_income_for_underage_person()
    {
        var menor = new Pessoa
        {
            DataNascimento = DateTime.Today.AddYears(-15)
        };
        var categoriaReceita = new Categoria { Finalidade = Categoria.EFinalidade.Receita };

        var tx = new Transacao
        {
            Descricao = "teste",
            Valor = 10m,
            Tipo = Tipo.Receita,
            Data = DateTime.Today
        };

        TransacaoNavigation.SetCategoria(tx, categoriaReceita);

        var act = () => TransacaoNavigation.SetPessoa(tx, menor);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Menores*");
    }

    [Fact(DisplayName = "should_allow_income_for_adult_person")]
    public void should_allow_income_for_adult_person()
    {
        var adulto = new Pessoa { DataNascimento = DateTime.Today.AddYears(-30) };
        var categoriaReceita = new Categoria { Finalidade = Categoria.EFinalidade.Receita };

        var tx = new Transacao
        {
            Descricao = "teste",
            Valor = 10m,
            Tipo = Tipo.Receita,
            Data = DateTime.Today
        };

        var act = () =>
        {
            TransacaoNavigation.SetCategoria(tx, categoriaReceita);
            TransacaoNavigation.SetPessoa(tx, adulto);
        };
        act.Should().NotThrow();
    }

    [Fact(DisplayName = "should_not_allow_receita_when_categoria_is_somente_despesa")]
    public void should_not_allow_receita_when_categoria_is_somente_despesa()
    {
        var categoriaDespesa = new Categoria { Finalidade = Categoria.EFinalidade.Despesa };

        var tx = new Transacao
        {
            Descricao = "teste",
            Valor = 10m,
            Tipo = Tipo.Receita,
            Data = DateTime.Today
        };

        var act = () => TransacaoNavigation.SetCategoria(tx, categoriaDespesa);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*receita em categoria de despesa*");
    }

    [Fact(DisplayName = "should_not_allow_despesa_when_categoria_is_somente_receita")]
    public void should_not_allow_despesa_when_categoria_is_somente_receita()
    {
        var categoriaReceita = new Categoria { Finalidade = Categoria.EFinalidade.Receita };

        var tx = new Transacao
        {
            Descricao = "teste",
            Valor = 10m,
            Tipo = Tipo.Despesa,
            Data = DateTime.Today
        };

        var act = () => TransacaoNavigation.SetCategoria(tx, categoriaReceita);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*despesa em categoria de receita*");
    }
}
