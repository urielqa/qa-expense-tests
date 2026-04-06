using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Integration.Contracts;
using IntegrationTests.Integration.Fixtures;
using IntegrationTests.Integration.Support;

namespace IntegrationTests.Integration.BusinessRules;

// Apagou a pessoa, as transações dela não podem ficar soltas no ar.
public sealed class PersonDeleteCascadeTests : IClassFixture<ExpenseApiFixture>
{
    private readonly HttpClient _client;

    public PersonDeleteCascadeTests(ExpenseApiFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact(DisplayName = "Deletar pessoa some as transações dela")]
    public async Task Deletar_pessoa_some_transacoes()
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var adulto = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-35);

        var rPessoa = await _client.PostAsJsonAsync(
            ApiRoutes.Pessoas,
            new CreatePessoaRequest($"IT_casc_{id}", adulto));
        rPessoa.StatusCode.Should().Be(HttpStatusCode.Created);
        var pessoa = await rPessoa.Content.ReadFromJsonAsync<PessoaResponse>(JsonDefaults.Options);
        pessoa.Should().NotBeNull();

        var rCat = await _client.PostAsJsonAsync(
            ApiRoutes.Categorias,
            new CreateCategoriaRequest($"IT_rec_{id}", FinalidadeCategoria.Receita));
        rCat.StatusCode.Should().Be(HttpStatusCode.Created);
        var cat = await rCat.Content.ReadFromJsonAsync<CategoriaResponse>(JsonDefaults.Options);
        cat.Should().NotBeNull();

        var rCria = await _client.PostAsJsonAsync(
            ApiRoutes.Transacoes,
            new CreateTransacaoRequest(
                $"IT_casc_tr_{id}",
                42,
                TipoTransacao.Receita,
                cat!.Id,
                pessoa!.Id,
                DateTimeOffset.UtcNow));
        rCria.StatusCode.Should().Be(HttpStatusCode.Created);
        var trans = await rCria.Content.ReadFromJsonAsync<TransacaoResponse>(JsonDefaults.Options);
        trans.Should().NotBeNull();

        var antes = await _client.GetAsync(ApiRoutes.Transacao(trans!.Id));
        antes.StatusCode.Should().Be(HttpStatusCode.OK, "antes de apagar a pessoa a transação tem que existir");

        var rDel = await _client.DeleteAsync(ApiRoutes.Pessoa(pessoa.Id));
        rDel.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var depois = await _client.GetAsync(ApiRoutes.Transacao(trans.Id));
        depois.StatusCode.Should().Be(
            HttpStatusCode.NotFound,
            because: "cascata: sumiu a pessoa, a transação não devia continuar acessível");
    }
}
