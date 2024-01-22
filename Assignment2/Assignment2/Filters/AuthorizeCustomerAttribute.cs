using System;
using McbaExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Assignment2.Filters;

public class AuthorizeCustomerAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Bonus Material: Implement global authorisation check.
        // Skip authorisation check if the [AllowAnonymous] attribute is present.
        // Another technique to perform the check: x.GetType() == typeof(AllowAnonymousAttribute)
        //if(context.ActionDescriptor.EndpointMetadata.Any(x => x is AllowAnonymousAttribute))
        //    return;

        var customerID = context.HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
        if (!customerID.HasValue)
            context.Result = new RedirectToActionResult("Index", "Home", null);
    }
}
