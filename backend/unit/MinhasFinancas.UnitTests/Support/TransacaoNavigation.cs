using System.Reflection;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.UnitTests.Support;

// Categoria e Pessoa têm setter internal — sem mexer no Domain, a saída foi reflexão.
// Fica aqui isolado pra não espalhar esse "hack" nos testes.
internal static class TransacaoNavigation
{
    public static void SetCategoria(Transacao transacao, Categoria categoria) =>
        SetNavigation(transacao, nameof(Transacao.Categoria), categoria);

    public static void SetPessoa(Transacao transacao, Pessoa pessoa) =>
        SetNavigation(transacao, nameof(Transacao.Pessoa), pessoa);

    private static void SetNavigation(Transacao transacao, string propertyName, object? value)
    {
        var prop = typeof(Transacao).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException($"Property {propertyName} not found.");
        var setter = prop.GetSetMethod(nonPublic: true)
            ?? throw new InvalidOperationException($"Setter for {propertyName} not found.");
        try
        {
            setter.Invoke(transacao, new[] { value });
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }
}
