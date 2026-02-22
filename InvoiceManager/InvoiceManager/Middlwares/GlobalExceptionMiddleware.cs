using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FluentValidation.ValidationException;

namespace InvoiceManager.Middlwares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            await HandleException(httpContext, e);
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task HandleException(HttpContext httpContext, Exception exception)
    {
        _logger.LogError(exception, exception.Message);
        
        httpContext.Response.ContentType = "application/json";
        ProblemDetails problemDetails;
        int statusCode;
        switch (exception)
        {
            case ValidationException validationException:
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                problemDetails = CreateValidationProblemDetails(httpContext,validationException ,statusCode);
                break;
            }
            case KeyNotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                problemDetails = CreateProblemDetails(httpContext, statusCode, exception.Message);
                break;
            case ArgumentException:
                statusCode = (int)HttpStatusCode.BadRequest;
                problemDetails = CreateProblemDetails(httpContext, statusCode, exception.Message);
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails = CreateProblemDetails(httpContext, statusCode, exception.Message);
                break;
        }
        httpContext.Response.StatusCode = statusCode;
        var json = JsonSerializer.Serialize(problemDetails);
        await httpContext.Response.WriteAsync(json);
    }

    private ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int statusCode,
        string exceptionMessage)
    {
        return new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Status = statusCode,
            Detail = exceptionMessage,
            Instance = httpContext.Request.Path
        };
    }

    private ProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ValidationException validationException,
        int statusCode)
    {
        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        var problem = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7807#section-3.1",
            Title = "One or more validation error occured",
            Status = statusCode,
            Detail = "See the 'errors' property for more details",
            Instance = httpContext.Request.Path
        };
        problem.Extensions["errors"] = errors;
        return problem;
    }
}