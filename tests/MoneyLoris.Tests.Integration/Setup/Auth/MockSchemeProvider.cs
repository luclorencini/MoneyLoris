using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MoneyLoris.Tests.Integration.Setup.Auth;
public class MockSchemeProvider : AuthenticationSchemeProvider
{
    public MockSchemeProvider(IOptions<AuthenticationOptions> options) : base(options)
    {
    }

    protected MockSchemeProvider(
        IOptions<AuthenticationOptions> options,
        IDictionary<string, AuthenticationScheme> schemes
    )
        : base(options, schemes)
    {
    }

#pragma warning disable CS8609 // Nullability of reference types in return type doesn't match overridden member.
    public override Task<AuthenticationScheme> GetSchemeAsync(string name)
#pragma warning restore CS8609 // Nullability of reference types in return type doesn't match overridden member.
    {
        AuthenticationScheme mockScheme = new(
            IdentityConstants.ApplicationScheme,
            IdentityConstants.ApplicationScheme,
            typeof(MockAuthenticationHandler)
        );
        return Task.FromResult(mockScheme);
    }
}
