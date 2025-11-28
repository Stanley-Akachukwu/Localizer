using FluentValidation;
using Lacalizer.Shared.Dtos;

namespace Lacalizer.WebAPI.Services.Validations;
public interface IValidationService
{
    Task<LocalizerApiResponse<TResponse>> ValidateAsync<TRequest, TResponse>(
    TRequest command,
    IValidator<TRequest> validator);
    Task<bool> IsValidEmail(string email);
}