using System.Security.Claims;

namespace MoneyLoris.Tests.Integration.Setup.Auth;
public class MockClaimSeed
{
    private readonly IEnumerable<Claim> _seed;

    public MockClaimSeed(IEnumerable<Claim> seed)
    {
        _seed = seed;
    }

    public IEnumerable<Claim> getSeeds() => _seed;
}
