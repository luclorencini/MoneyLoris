using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MoneyLoris.Tests.Integration.Setup.Auth;
public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly MockClaimSeed _claimSeed;

    public MockAuthenticationHandler(
        MockClaimSeed claimSeed,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    )
        : base(options, logger, encoder, clock)
    {
        _claimSeed = claimSeed;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claimsIdentity = new ClaimsIdentity(_claimSeed.getSeeds(), IdentityConstants.ApplicationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var ticket = new AuthenticationTicket(claimsPrincipal, IdentityConstants.ApplicationScheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
