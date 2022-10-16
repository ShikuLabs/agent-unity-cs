namespace ICAgentFFI.Tests.Agent;

using ICAgentFFI.Agent;

public class HttpAgentTest
{
    [Fact]
    public void Create_ShouldWork()
    {
        var mainNet = new string("https://ic0.app");
        HttpAgent.Create(mainNet);
    }
}