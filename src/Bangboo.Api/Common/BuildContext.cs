using System.Reflection;

namespace Bangboo.Api.Common;

public static class BuildContext
{
    /// <summary>
    /// <c>true</c> quando o app está sendo executado pelo build só para gerar o OpenAPI
    /// (Microsoft.Extensions.ApiDescription.Server). Nesse modo não há configuração/segredos.
    /// </summary>
    public static bool IsGeneratingOpenApi { get; } =
        Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider";
}
