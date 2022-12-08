using ICAgentFFI.Candid;

namespace ICAgentFFI.Tests.Candid;

public class IDLArgsTest
{
    static string IdlArgsText = "(true, principal \"2vxsx-fae\", -12 : int32)";
    static byte[] IdlArgsBytes = {
        68, 73, 68, 76, 0, 3, 126, 104, 117, 1, 1, 1, 4, 244, 255, 255, 255,
    };

    [Fact]
    public void FromText_ShouldWork()
    {
        IDLArgs.FromText("(128: nat64)");
        IDLArgs.FromText("(principal \"2vxsx-fae\")");
    }

    [Fact]
    public void ToString_ShouldWork()
    {
        var args01 = IDLValue.FromText("(128: nat64)");
        Assert.Equal("128 : nat64", args01.ToString());

        var args02 = IDLValue.FromText("(principal \"2vxsx-fae\")");
        Assert.Equal("principal \"2vxsx-fae\"", args02.ToString());
    }

    [Fact]
    public void FromBytes_shouldWork()
    {
        var args = IDLArgs.FromBytes(IdlArgsBytes);

        Assert.Equal(IdlArgsText, args.ToString());
    }

    [Fact]
    public void ToBytes_shouldWork()
    {
        var args = IDLArgs.FromText(IdlArgsText);

        Assert.Equal(IdlArgsBytes, args.ToBytes());
    }
    
    [Fact]
    void WithVec_ShouldWork()
    {
        IDLValue[] values = new[]
        {
            IDLValue.WithBool(true),
            IDLValue.WithNull(),
            IDLValue.WithPrincipal(Principal.Anonymous())
        };
        var args = IDLArgs.WithVec(values);

        var vec = args.AsVec();

        for (int i = 0; i < values.Length; i++)
        {
            Assert.Equal(values[i], vec[i]);
        }
    }

    [Fact]
    public void AsVec_shouldWork()
    {
        var args = IDLArgs.FromBytes(IdlArgsBytes);

        var vals = args.AsVec();

        Assert.True(vals[0].AsBool());
        Assert.Equal(Principal.Anonymous(), vals[1].AsPrincipal());
        Assert.Equal(-12, vals[2].AsInt32());
    }
}