namespace Bangboo.Api.Common.Errors;

/// <summary>
/// Corpo padrão de TODA resposta de erro da API.
/// </summary>
/// <param name="Status">Código HTTP (repetido no corpo para facilitar o consumo no front).</param>
/// <param name="Message">Mensagem legível, segura para exibir ao usuário.</param>
public sealed record ApiError(int Status, string Message);
