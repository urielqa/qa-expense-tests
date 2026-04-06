using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Integration.Contracts;
using IntegrationTests.Integration.Fixtures;
using IntegrationTests.Integration.Support;

namespace IntegrationTests.Integration.BusinessRules;

// Finalidade "ambos" = pode receita e despesa na mesma categoria, senão quebra o caso do meio.
public sealed class AmbosCategoryTests : IClassFixture<ExpenseApiFixture>
{
    private readonly HttpClient _client;

    public AmbosCategoryTests(ExpenseApiFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact(DisplayName = "Categoria ambos aceita receita e despesa")]
    public async Task Categoria_ambos_aceita_os_dois_tipos()
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var adulto = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-28);

        var rPessoa = await _client.PostAsJsonAsync(
            ApiRoutes.Pessoas,
            new CreatePessoaRequest($"IT_amb_{id}", adulto));
        rPessoa.StatusCode.Should().Be(HttpStatusCode.Created);
        var pessoa = await rPessoa.Content.ReadFromJsonAsync<PessoaResponse>(JsonDefaults.Options);
        pessoa.Should().NotBeNull();

        var rCat = await _client.PostAsJsonAsync(
            ApiRoutes.Categorias,
            new CreateCategoriaRequest($"IT_ambcat_{id}", FinalidadeCategoria.Ambos));
        rCat.StatusCode.Should().Be(HttpStatusCode.Created);
        var cat = await rCat.Content.ReadFromJsonAsync<CategoriaResponse>(JsonDefaults.Options);
        cat.Should().NotBeNull();

        var rRec = await _client.PostAsJsonAsync(
            ApiRoutes.Transacoes,
            new CreateTransacaoRequest(
                $"IT_amb_rec_{id}",
                100,
                TipoTransacao.Receita,
                cat!.Id,
                pessoa!.Id,
                DateTimeOffset.UtcNow));

        var rDesp = await _client.PostAsJsonAsync(
            ApiRoutes.Transacoes,
            new CreateTransacaoRequest(
                $"IT_amb_desp_{id}",
                30,
                TipoTransacao.Despesa,
                cat.Id,
                pessoa.Id,
                DateTimeOffset.UtcNow));

        rRec.StatusCode.Should().Be(HttpStatusCode.Created);
        rDesp.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verifica que Valor retornado preserva casas decimais (decimal, não double)
        var transacaoRec = await rRec.Content.ReadFromJsonAsync<TransacaoResponse>(JsonDefaults.Options);
        transacaoRec.Should().NotBeNull();
        transacaoRec!.Valor.Should().Be(100m, "valor deve ser decimal preciso, sem perda de ponto flutuante");

        var transacaoDesp = await rDesp.Content.ReadFromJsonAsync<TransacaoResponse>(JsonDefaults.Options);
        transacaoDesp.Should().NotBeNull();
        transacaoDesp!.Valor.Should().Be(30m);
    }
}
