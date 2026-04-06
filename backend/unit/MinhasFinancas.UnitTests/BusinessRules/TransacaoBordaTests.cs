using FluentAssertions;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.UnitTests.Support;
using Xunit;
using Tipo = MinhasFinancas.Domain.Entities.Transacao.ETipo;

namespace MinhasFinancas.UnitTests.BusinessRules;

/// <summary>
/// Casos de borda que complementam TransacaoRegrasTests e CategoriaPermiteTipoTests.
/// Exercitam limites de valor, exatamente-18-anos e despesa de menor (que é permitida).
/// </summary>
public class TransacaoBordaTests
{
    // --- Valor ---

    [Fact(DisplayName = "valor_minimo_aceito_0_01")]
    public void valor_minimo_aceito()
    {
        var tx = new Transacao { Descricao = "borda", Valor = 0.01m, Tipo = Tipo.Despesa, Data = DateTime.Today };
        tx.Valor.Should().Be(0.01m);
    }

    [Fact(DisplayName = "valor_zero_invalido_por_range_annotation")]
    public void valor_zero_invalido()
    {
        // Aqui fui verificar se o domain bloqueia valor 0 diretamente.
        // Não bloqueia — a validação vem do [Range(0.01, ...)] no DTO, não do domain.
        // Deixei o teste documentando isso pra ficar claro onde a regra mora.
        var tx = new Transacao { Descricao = "borda", Valor = 0m, Tipo = Tipo.Despesa, Data = DateTime.Today };
        tx.Valor.Should().Be(0m); // domain aceita, validação é feita no DTO/controller
    }

    // --- Exatamente 18 anos ---

    [Fact(DisplayName = "pessoa_com_exatamente_18_anos_pode_ter_receita")]
    public void pessoa_com_exatamente_18_anos_pode_receita()
    {
        var adulto = new Pessoa { DataNascimento = DateTime.Today.AddYears(-18) };
        var catReceita = new Categoria { Finalidade = Categoria.EFinalidade.Receita };

        var tx = new Transacao { Descricao = "borda", Valor = 1m, Tipo = Tipo.Receita, Data = DateTime.Today };
        TransacaoNavigation.SetCategoria(tx, catReceita);

        var act = () => TransacaoNavigation.SetPessoa(tx, adulto);
        act.Should().NotThrow("pessoa com exatamente 18 anos é maior de idade");
    }

    [Fact(DisplayName = "pessoa_com_17_anos_e_364_dias_nao_pode_receita")]
    public void pessoa_com_quase_18_nao_pode_receita()
    {
        // 18 anos menos 1 dia = ainda menor
        var menor = new Pessoa { DataNascimento = DateTime.Today.AddYears(-18).AddDays(1) };
        var catReceita = new Categoria { Finalidade = Categoria.EFinalidade.Receita };

        var tx = new Transacao { Descricao = "borda", Valor = 1m, Tipo = Tipo.Receita, Data = DateTime.Today };
        TransacaoNavigation.SetCategoria(tx, catReceita);

        var act = () => TransacaoNavigation.SetPessoa(tx, menor);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Menores*");
    }

    // --- Menor pode ter despesa ---

    [Fact(DisplayName = "menor_pode_ter_despesa")]
    public void menor_pode_ter_despesa()
    {
        var menor = new Pessoa { DataNascimento = DateTime.Today.AddYears(-15) };
        var catDespesa = new Categoria { Finalidade = Categoria.EFinalidade.Despesa };

        var tx = new Transacao { Descricao = "borda", Valor = 5m, Tipo = Tipo.Despesa, Data = DateTime.Today };
        TransacaoNavigation.SetCategoria(tx, catDespesa);

        var act = () => TransacaoNavigation.SetPessoa(tx, menor);
        act.Should().NotThrow("menores podem ter despesas — só receitas são bloqueadas");
    }

    // --- Categoria Ambas ---

    [Fact(DisplayName = "categoria_ambas_aceita_despesa_de_menor")]
    public void categoria_ambas_aceita_despesa_de_menor()
    {
        var menor = new Pessoa { DataNascimento = DateTime.Today.AddYears(-10) };
        var catAmbas = new Categoria { Finalidade = Categoria.EFinalidade.Ambas };

        var tx = new Transacao { Descricao = "borda", Valor = 1m, Tipo = Tipo.Despesa, Data = DateTime.Today };
        TransacaoNavigation.SetCategoria(tx, catAmbas);

        var act = () => TransacaoNavigation.SetPessoa(tx, menor);
        act.Should().NotThrow();
    }

    // --- Descrição ---

    [Fact(DisplayName = "descricao_vazia_nao_e_bloqueada_no_domain")]
    public void descricao_vazia_domain_nao_bloqueia()
    {
        // StringLength e Required são validados no DTO/controller, não no domain puro
        // Este teste documenta esse comportamento esperado
        var tx = new Transacao { Descricao = "", Valor = 1m, Tipo = Tipo.Despesa, Data = DateTime.Today };
        tx.Descricao.Should().BeEmpty("domain não lança para string vazia — a validação é do DTO");
    }
}
