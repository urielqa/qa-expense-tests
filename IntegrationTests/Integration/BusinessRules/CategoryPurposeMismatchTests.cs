using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Integration.Contracts;
using IntegrationTests.Integration.Fixtures;
using IntegrationTests.Integration.Support;

namespace IntegrationTests.Integration.BusinessRules;

// Categoria só despesa não pode virar receita na mão — finalidade tem que bater com o tipo.
public sealed class CategoryPurposeMismatchTests : IClassFixture<ExpenseApiFixture>
{
    private readonly HttpClient _client;

    public CategoryPurposeMismatchTests(ExpenseApiFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact(DisplayName = "Receita com categoria só despesa não entra")]
    public async Task Receita_com_categoria_so_despesa_nao_entra()
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var adulto = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-30);

        var rPessoa = await _client.PostAsJsonAsync(
            ApiRoutes.Pessoas,
            new CreatePessoaRequest($"IT_adulto_{id}", adulto));
        rPessoa.StatusCode.Should().Be(HttpStatusCode.Created);
        var pessoa = await rPessoa.Content.ReadFromJsonAsync<PessoaResponse>(JsonDefaults.Options);
        pessoa.Should().NotBeNull();

        var rCat = await _client.PostAsJsonAsync(
            ApiRoutes.Categorias,
            new CreateCategoriaRequest($"IT_sodesp_{id}", FinalidadeCategoria.Despesa));
        rCat.StatusCode.Should().Be(HttpStatusCode.Created);
        var cat = await rCat.Content.ReadFromJsonAsync<CategoriaResponse>(JsonDefaults.Options);
        cat.Should().NotBeNull();

        var trans = new CreateTransacaoRequest(
            $"IT_mix_{id}",
            25,
            TipoTransacao.Receita,
            cat!.Id,
            pessoa!.Id,
            DateTimeOffset.UtcNow);

        var rTrans = await _client.PostAsJsonAsync(ApiRoutes.Transacoes, trans);

        rTrans.StatusCode.Should().Be(
            HttpStatusCode.BadRequest,
            because: "mistura de tipo e finalidade não pode passar como 201");

        var problem = await rTrans.Content.ReadFromJsonAsync<ProblemDetails>(JsonDefaults.Options);
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(400);
        problem.Title.Should().NotBeNullOrWhiteSpace();
    }
}
