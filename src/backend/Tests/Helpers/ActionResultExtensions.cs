using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UnitTests.Helpers;

public static class ActionResultExtensions
{
    extension<TResponseType>(ActionResult<TResponseType> actionResult)
    {
        public void AssertResponseReturnEquals(TResponseType expected)
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

        public void AssertResponseStatusCode(HttpStatusCode expectedStatusCode)
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
    }

    extension(ActionResult actionResult)
    {
        public void AssertResponseStatusCode(HttpStatusCode expectedStatusCode)
        {
            if (actionResult == null)
            {
                throw new ArgumentNullException(nameof(actionResult), "ActionResult cannot be null");
            }
        
            int expectedStatusCodeValue = (int)expectedStatusCode;
            int actualStatusCode = GetStatusCodeFromActionResult(actionResult);
        
            if (expectedStatusCodeValue != actualStatusCode)
            {
                throw new InvalidOperationException($"Expected statusEnum code {expectedStatusCodeValue} ({expectedStatusCode}), but got {actualStatusCode}");
            }
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