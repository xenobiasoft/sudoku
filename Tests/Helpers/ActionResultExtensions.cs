using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UnitTests.Helpers;

public static class ActionResultExtensions
{
    public static void AssertResponseReturnEquals<TResponseType>(this ActionResult<TResponseType> result, TResponseType expected)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result), "ActionResult cannot be null");
        }
        var returnValue = DetermineResponseReturnValue<TResponseType>(result);
        
        if (returnValue == null)
        {
            throw new InvalidOperationException("The response return value is null");
        }

        returnValue.Should().BeEquivalentTo(expected, options => options.ExcludingMissingMembers());
    }

    public static void AssertResponseStatusCode<TResponseType>(this ActionResult<TResponseType> response, HttpStatusCode statusCode)
    {
        var actionResult = response.Result;
        
        if (actionResult == null)
        {
            throw new ArgumentNullException(nameof(response.Result), "ActionResult cannot be null");
        }

        var expectedStatusCode = (int)statusCode;
        int actualStatusCode;

        if (actionResult is ObjectResult objectResult)
        {
            actualStatusCode = objectResult.StatusCode ?? 200;
        }
        else if (actionResult is StatusCodeResult statusCodeResult)
        {
            actualStatusCode = statusCodeResult.StatusCode;
        }
        else
        {
            actualStatusCode = DetermineStatusCode(actionResult);
        }
        
        if (expectedStatusCode != actualStatusCode)
        {
            throw new InvalidOperationException($"Expected status code {expectedStatusCode} ({statusCode}), but got {actualStatusCode}");
        }
    }

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