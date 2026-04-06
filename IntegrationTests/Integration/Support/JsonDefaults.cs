using System.Text.Json;

namespace IntegrationTests.Integration.Support;

// JSON case-insensitive pra não brigar com maiúscula/minúscula no retorno.
internal static class JsonDefaults
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
