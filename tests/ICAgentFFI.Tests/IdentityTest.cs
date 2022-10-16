namespace ICAgentFFI.Tests;

public class IdentityTest
{
    [Fact]
    public void Anonymous_ShouldWork()
    {
        var identity = Identity.Anonymous();
        var principal = identity.Sender();
        
        Assert.Equal(principal, Principal.Anonymous());
    }
    
    [Fact]
    public void BasicRandom_ShouldWork()
    {
        var identity = Identity.BasicRandom();
        
        Assert.Equal(IdentityType.Basic, identity.Type);
    }
    
    [Fact]
    public void Secp256k1Random_ShouldWork()
    {
        var identity = Identity.Secp256k1Random();
        
        Assert.Equal(IdentityType.Secp256K1, identity.Type);
    }
}