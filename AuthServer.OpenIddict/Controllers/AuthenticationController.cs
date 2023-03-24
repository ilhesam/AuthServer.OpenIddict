using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace AuthServer.OpenIddict.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost("~/connect/token")]
        public IActionResult Exchange()
        {
            var oidcRequest = HttpContext.GetOpenIddictServerRequest();

            if (oidcRequest?.IsPasswordGrantType() is true)
            {
                // Validate the user credentials.
                if (oidcRequest.Username != "admin" && oidcRequest.Password != "1qaz!QAZ")
                {
                    return BadRequest(new OpenIddictResponse
                    {
                        Error = OpenIddictConstants.Errors.InvalidRequest
                    });
                }

                // Create a new ClaimsIdentity holding the user identity.
                var identity = new ClaimsIdentity(OpenIddictConstants.Schemes.Bearer, OpenIddictConstants.Claims.Name, OpenIddictConstants.Claims.Role);

                // Add a "sub" claim containing the user identifier, and attach
                // the "access_token" destination to allow OpenIddict to store it
                // in the access token, so it can be retrieved from your controllers.
                identity.AddClaim(OpenIddictConstants.Claims.Subject, "71346D62-9BA5-4B6D-9ECA-755574D628D8", OpenIddictConstants.Destinations.AccessToken);
                identity.AddClaim(OpenIddictConstants.Claims.Name, "Hesam", OpenIddictConstants.Destinations.AccessToken);

                var principal = new ClaimsPrincipal(identity);

                // Ask OpenIddict to generate a new token and return an OAuth2 token response.
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.UnsupportedGrantType
            });
        }
    }
}
