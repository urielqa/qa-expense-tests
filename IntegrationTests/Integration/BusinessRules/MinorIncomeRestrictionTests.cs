using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Integration.Contracts;
using IntegrationTests.Integration.Fixtures;
using IntegrationTests.Integration.Support;

namespace IntegrationTests.Integration.BusinessRules;

// Menor não pode lançar receita — regra fechada no escopo.
public sealed class MinorIncomeRestrictionTests : IClassFixture<ExpenseApiFixture>
{
    private readonly HttpClient _client;

    public MinorIncomeRestrictionTests(ExpenseApiFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact(DisplayName = "Menor não pode criar receita")]
    public async Task Menor_nao_pode_criar_receita()
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var nascimento = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-15);

        var respPessoa = await _client.PostAsJsonAsync(
            ApiRoutes.Pessoas,
            new CreatePessoaRequest($"IT_menor_{id}", nascimento));
        respPessoa.StatusCode.Should().Be(HttpStatusCode.Created, "precisa dar 201 na pessoa, senão o teste não faz sentido");
        var pessoa = await respPessoa.Content.ReadFromJsonAsync<PessoaResponse>(JsonDefaults.Options);
        pessoa.Should().NotBeNull();
        pessoa!.Idade.Should().BeLessThan(18, "sanity check — tem que ser menor mesmo");

        var respCat = await _client.PostAsJsonAsync(
            ApiRoutes.Categorias,
            new CreateCategoriaRequest($"IT_rec_{id}", FinalidadeCategoria.Receita));
        respCat.StatusCode.Should().Be(HttpStatusCode.Created);
        var cat = await respCat.Content.ReadFromJsonAsync<CategoriaResponse>(JsonDefaults.Options);
        cat.Should().NotBeNull();

        var transacao = new CreateTransacaoRequest(
            $"IT_rec_menor_{id}",
            100.50m,
            TipoTransacao.Receita,
            cat!.Id,
            pessoa.Id,
            DateTimeOffset.UtcNow);

        var respTrans = await _client.PostAsJsonAsync(ApiRoutes.Transacoes, transacao);

        // Aqui o certo é 400. 201 = aceitou receita de menor (ruim). 500 = estourou em vez de validar.
        respTrans.StatusCode.Should().Be(
            HttpStatusCode.BadRequest,
            because: "API devia barrar receita de menor");

        var problem = await respTrans.Content.ReadFromJsonAsync<ProblemDetails>(JsonDefaults.Options);
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(400);
        problem.Title.Should().NotBeNullOrWhiteSpace();
    }
}
