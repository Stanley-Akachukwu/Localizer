using FluentValidation;
using Lacalizer.Shared.Dtos;
using Lacalizer.WebAPI.Infrastructure;
using MediatR;
using System.Net.Mail;

namespace Lacalizer.WebAPI.Services.Validations;

public class ValidationError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ValidationService : IValidationService
{
    private readonly LocalizeDbContext _dbContext;

    public ValidationService(LocalizeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Task.FromResult(false);

        try
        {
            var addr = new MailAddress(email);
            return Task.FromResult(addr.Address == email);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<LocalizerApiResponse<TResponse>> ValidateAsync<TRequest, TResponse>(
        TRequest command,
        IValidator<TRequest> validator)
    {
        if (command == null)
            return LocalizerApiResponse<TResponse>.Failure(
                "Validation target cannot be null.",
                StatusCodes.Status400BadRequest);

        if (validator == null)
            return LocalizerApiResponse<TResponse>.Failure(
                $"No validator provided for {typeof(TRequest).Name}.",
                StatusCodes.Status500InternalServerError);

        var validationResult = await validator.ValidateAsync(command)
                                              .ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => new ValidationError
                    {
                        Code = e.ErrorCode,
                        Message = e.ErrorMessage
                    }).ToList()
                );

            return LocalizerApiResponse<TResponse>.Failure(
                errors, StatusCodes.Status400BadRequest);
        }

        return LocalizerApiResponse<TResponse>.Success(
            default!, StatusCodes.Status200OK);
    }
}

public static class MediatorValidationHelper
{
    public static async Task<LocalizerApiResponse<TResponse>> ExecuteAsync<TCommand, TResponse>(
        TCommand command,
        IValidator<TCommand> validator,
        IValidationService validationService,
        IMediator mediator)
        where TCommand : IRequest<LocalizerApiResponse<TResponse>>
    {
        var validationResult = await validationService
            .ValidateAsync<TCommand, TResponse>(command, validator);

        if (!validationResult.IsSuccess)
            return validationResult;

        return await mediator.Send(command);
    }
}
