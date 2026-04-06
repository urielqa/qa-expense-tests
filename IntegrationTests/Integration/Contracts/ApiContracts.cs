namespace IntegrationTests.Integration.Contracts;

// Números iguais ao Swagger (finalidade / tipo).
internal enum FinalidadeCategoria
{
    Receita = 0,
    Despesa = 1,
    Ambos = 2
}

internal enum TipoTransacao
{
    Receita = 0,
    Despesa = 1
}

internal sealed record CreatePessoaRequest(string Nome, DateOnly DataNascimento);

internal sealed record PessoaResponse(Guid Id, string? Nome, DateTime DataNascimento, int Idade);

internal sealed record CreateCategoriaRequest(string Descricao, FinalidadeCategoria Finalidade);

internal sealed record CategoriaResponse(Guid Id, string? Descricao, FinalidadeCategoria Finalidade);

internal sealed record CreateTransacaoRequest(
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    Guid CategoriaId,
    Guid PessoaId,
    DateTimeOffset Data);

internal sealed record TransacaoResponse(
    Guid Id,
    string? Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    Guid CategoriaId,
    string? CategoriaDescricao,
    Guid PessoaId,
    string? PessoaNome,
    DateTime Data);

internal sealed record ProblemDetails(
    string? Type,
    string? Title,
    int? Status,
    string? Detail,
    string? Instance);
