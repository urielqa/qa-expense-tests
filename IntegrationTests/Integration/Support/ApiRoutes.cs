namespace IntegrationTests.Integration.Support;

// Caminhos da API v1 — um lugar só pra não ficar string jogada no teste.
internal static class ApiRoutes{
    public const string ApiVersion = "v1";

    public static string Pessoas => $"api/{ApiVersion}/Pessoas";
    public static string Pessoa(Guid id) => $"{Pessoas}/{id}";
    public static string Categorias => $"api/{ApiVersion}/Categorias";
    public static string Transacoes => $"api/{ApiVersion}/Transacoes";
    public static string Transacao(Guid id) => $"{Transacoes}/{id}";
}
