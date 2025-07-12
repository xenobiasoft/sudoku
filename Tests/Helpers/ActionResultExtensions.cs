using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UnitTests.Helpers;

/// <summary>
/// Extension methods for testing ASP.NET Core ActionResult responses.
/// </summary>
public static class ActionResultExtensions
{
    /// <summary>
    /// Asserts that the action result value is equivalent to the expected value.
    /// </summary>
    /// <typeparam name="TResponseType">The type of the response.</typeparam>
    /// <param name="actionResult">The action result to verify.</param>
    /// <param name="expected">The expected value.</param>
    /// <exception cref="ArgumentNullException">Thrown when actionResult is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the response value is null or of unsupported type.</exception>
    public static void AssertResponseReturnEquals<TResponseType>(this ActionResult<TResponseType> actionResult, TResponseType expected)
    {
        if (actionResult == null)
        {
            throw new ArgumentNullException(nameof(actionResult), "ActionResult cannot be null");
        }
        
        var returnValue = DetermineResponseReturnValue<TResponseType>(actionResult);
        
        if (returnValue == null)
        {
            throw new InvalidOperationException("The response return value is null");
        }

        returnValue.Should().BeEquivalentTo(expected, options => options.ExcludingMissingMembers());
    }

    /// <summary>
    /// Asserts that the action result has the expected HTTP status code.
    /// </summary>
    /// <param name="actionResult">The action result to verify.</param>
    /// <param name="expectedStatusCode">The expected HTTP status code.</param>
    /// <exception cref="ArgumentNullException">Thrown when actionResult is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the status code doesn't match.</exception>
    public static void AssertResponseStatusCode(this ActionResult actionResult, HttpStatusCode expectedStatusCode)
    {
        if (actionResult == null)
        {
            throw new ArgumentNullException(nameof(actionResult), "ActionResult cannot be null");
        }
        
        int expectedStatusCodeValue = (int)expectedStatusCode;
        int actualStatusCode = GetStatusCodeFromActionResult(actionResult);
        
        if (expectedStatusCodeValue != actualStatusCode)
        {
            throw new InvalidOperationException($"Expected status code {expectedStatusCodeValue} ({expectedStatusCode}), but got {actualStatusCode}");
        }
    }

    /// <summary>
    /// Asserts that the generic action result has the expected HTTP status code.
    /// </summary>
    /// <typeparam name="TResponseType">The type of the response.</typeparam>
    /// <param name="actionResult">The action result to verify.</param>
    /// <param name="expectedStatusCode">The expected HTTP status code.</param>
    /// <exception cref="ArgumentNullException">Thrown when actionResult or its Result property is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the status code doesn't match.</exception>
    public static void AssertResponseStatusCode<TResponseType>(this ActionResult<TResponseType> actionResult, HttpStatusCode expectedStatusCode)
    {
        if (actionResult == null)
        {
            throw new ArgumentNullException(nameof(actionResult), "ActionResult cannot be null");
        }
        
        var innerResult = actionResult.Result;
        
        if (innerResult == null)
        {
            throw new ArgumentNullException(nameof(actionResult.Result), "ActionResult.Result cannot be null");
        }

        AssertResponseStatusCode(innerResult, expectedStatusCode);
    }

    /// <summary>
    /// Extracts the response value from the action result.
    /// </summary>
    private static TResponseType DetermineResponseReturnValue<TResponseType>(ActionResult<TResponseType> result)
    {
        return result.Result switch
        {
            OkObjectResult ok => (TResponseType)ok.Value!,
            CreatedAtActionResult created => (TResponseType)created.Value!,
            ObjectResult obj => (TResponseType)obj.Value!,
            _ => throw new InvalidOperationException($"Unsupported result type: {result.Result?.GetType().Name}")
        };
    }

    /// <summary>
    /// Gets the HTTP status code from an action result.
    /// </summary>
    private static int GetStatusCodeFromActionResult(ActionResult actionResult)
    {
        if (actionResult is ObjectResult objectResult)
        {
            return objectResult.StatusCode ?? 200;
        }
        
        if (actionResult is StatusCodeResult statusCodeResult)
        {
            return statusCodeResult.StatusCode;
        }
        
        return DetermineStatusCode(actionResult);
    }

    /// <summary>
    /// Maps specific ActionResult types to their corresponding HTTP status codes.
    /// </summary>
    private static int DetermineStatusCode(ActionResult actionResult)
    {
        return actionResult switch
        {
            OkResult or OkObjectResult => 200,
            CreatedResult or CreatedAtActionResult or CreatedAtRouteResult => 201,
            AcceptedResult or AcceptedAtActionResult or AcceptedAtRouteResult => 202,
            NoContentResult => 204,
            BadRequestResult or BadRequestObjectResult => 400,
            UnauthorizedResult => 401,
            ForbidResult => 403,
            NotFoundResult or NotFoundObjectResult => 404,
            ConflictResult or ConflictObjectResult => 409,
            _ => 200 // Default to 200 OK for other result types
        };
    }
}