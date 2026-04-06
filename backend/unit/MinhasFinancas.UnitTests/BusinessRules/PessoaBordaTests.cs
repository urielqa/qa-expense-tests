using FluentAssertions;
using MinhasFinancas.Domain.Entities;
using Xunit;

namespace MinhasFinancas.UnitTests.BusinessRules;

/// <summary>
/// Casos de borda para Pessoa — CalcularIdade e EhMaiorDeIdade.
/// Não usa reflexão: Pessoa não tem setter internal nos campos relevantes.
/// </summary>
public class PessoaBordaTests
{
    [Fact(DisplayName = "pessoa_nascida_hoje_tem_idade_zero")]
    public void pessoa_nascida_hoje_tem_idade_zero()
    {
        var p = new Pessoa { DataNascimento = DateTime.Today, Nome = "borda" };
        p.Idade.Should().Be(0);
        p.EhMaiorDeIdade().Should().BeFalse();
    }

    [Fact(DisplayName = "pessoa_com_17_anos_completos_nao_eh_maior")]
    public void pessoa_com_17_anos_nao_eh_maior()
    {
        var p = new Pessoa { DataNascimento = DateTime.Today.AddYears(-17), Nome = "borda" };
        p.Idade.Should().Be(17);
        p.EhMaiorDeIdade().Should().BeFalse();
    }

    // Esse aqui eu quis ter certeza: se o aniversário é amanhã, ainda é menor hoje.
    // O cálculo de idade no domain usa DateTime.Today, então o boundary importa.
    [Fact(DisplayName = "aniversario_ainda_nao_ocorreu_este_ano_nao_conta")]
    public void aniversario_futuro_nao_conta()
    {
        var nasc = DateTime.Today.AddYears(-18).AddDays(1);
        var p = new Pessoa { DataNascimento = nasc, Nome = "borda" };
        p.Idade.Should().Be(17, "aniversário é amanhã");
        p.EhMaiorDeIdade().Should().BeFalse();
    }

    [Fact(DisplayName = "aniversario_ontem_conta_18_anos")]
    public void aniversario_ontem_conta()
    {
        var nasc = DateTime.Today.AddYears(-18).AddDays(-1);
        var p = new Pessoa { DataNascimento = nasc, Nome = "borda" };
        p.Idade.Should().Be(18);
        p.EhMaiorDeIdade().Should().BeTrue();
    }

    [Fact(DisplayName = "nome_nao_e_validado_no_domain")]
    public void nome_string_vazio_domain_aceita()
    {
        // Validação de nome fica no DTO ([Required]), não no domain
        var p = new Pessoa { Nome = "", DataNascimento = DateTime.Today.AddYears(-30) };
        p.Nome.Should().BeEmpty("domain não lança para nome vazio — validação é do DTO");
    }
}
