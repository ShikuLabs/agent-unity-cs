namespace ICAgentFFI.Tests;

public class PrincipalTest
{
    [Fact]
    public void ManagementCanister_ShouldWork()
    {
        Config.Reset();

        var BYTES_EXPECTED = Array.Empty<byte>();

        var principal = Principal.ManagementCanister();

        Assert.Equal(principal.Bytes, BYTES_EXPECTED);
    }

    [Fact]
    public void SelfAuthenticating_ShouldWork()
    {
        Config.Reset();

        byte[] PUBLIC_KEY =
        {
            0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22,
            0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44,
            0x33, 0x22, 0x11, 0x00,
        };

        byte[] BYTES_EXPECTED =
        {
            0x2f, 0x8e, 0x47, 0x38, 0xf9, 0xd7, 0x68, 0x16, 0x82, 0x99, 0x85, 0x41, 0x52, 0x67,
            0x86, 0x38, 0x07, 0xd3, 0x7d, 0x20, 0x6a, 0xd9, 0x0f, 0xea, 0x72, 0xbf, 0x9d, 0xcf,
            0x02,
        };

        var principal = Principal.SelfAuthenticating(PUBLIC_KEY);

        Assert.Equal(principal.Bytes, BYTES_EXPECTED);
    }

    [Fact]
    public void SelfAuthenticating_DataOverflow()
    {
        Config.Reset();
        Config.OutArrSize = 0;

        byte[] PUBLIC_KEY =
        {
            0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22,
            0x11, 0x00, 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44,
            0x33, 0x22, 0x11, 0x00,
        };

        Assert.Throws<DataOverflowException>(() => Principal.SelfAuthenticating(PUBLIC_KEY));
    }

    [Fact]
    public void Anonymous_ShouldWork()
    {
        Config.Reset();

        byte[] BYTES_EXPECTED = { 4 };

        var principal = Principal.Anonymous();

        Assert.Equal(principal.Bytes, BYTES_EXPECTED);
    }

    [Fact]
    public void Anonymous_DataOverflow()
    {
        Config.Reset();
        Config.OutArrSize = 0;

        Assert.Throws<DataOverflowException>(() => Principal.Anonymous());
    }

    [Fact]
    public void FromBytes_ShouldWork()
    {
        Config.Reset();

        byte[] BYTES = new byte[16];

        var principal = Principal.FromBytes(BYTES);

        Assert.Equal(principal.Bytes, BYTES);
    }

    [Fact]
    public void FromBytes_DataOverflow()
    {
        Config.Reset();
        Config.OutArrSize = 0;

        var BYTES = new byte[16];

        Assert.Throws<DataOverflowException>(() => Principal.FromBytes(BYTES));
    }

    [Fact]
    public void FromText_ShouldWork()
    {
        Config.Reset();

        var ANONYMOUS_TEXT = new string("2vxsx-fae");
        byte[] BYTES_EXPECTED = { 4 };

        var principal = Principal.FromText(ANONYMOUS_TEXT);

        Assert.Equal(principal.Bytes, BYTES_EXPECTED);
    }

    [Fact]
    public void FromText_DataOverflow()
    {
        Config.Reset();
        Config.OutArrSize = 0;

        var ANONYMOUS_TEXT = new string("2vxsx-fae");

        Assert.Throws<DataOverflowException>(() => Principal.FromText(ANONYMOUS_TEXT));
    }

    [Fact]
    public void ToText_ShouldWork()
    {
        Config.Reset();

        byte[] ANONYMOUS_BYTES = { 4 };
        var TEXT_EXPECTED = new string("2vxsx-fae");

        var principal = Principal.FromBytes(ANONYMOUS_BYTES);

        Assert.Equal(principal.ToString(), TEXT_EXPECTED);
    }

    [Fact]
    public void ToText_DataOverflow()
    {
        Config.Reset();
        Config.OutTextSize = 0;

        byte[] ANONYMOUS_BYTES = { 4 };
        var principal = Principal.FromBytes(ANONYMOUS_BYTES);

        Assert.Throws<DataOverflowException>(() => principal.ToString());
    }

    [Fact]
    public void Equal_ShouldWork()
    {
        Config.Reset();

        var anonymous01 = Principal.Anonymous();
        var anonymous02 = Principal.Anonymous();
        Assert.Equal(anonymous01, anonymous02);

        var managementCanister = Principal.ManagementCanister();
        Assert.NotEqual(anonymous01, managementCanister);
    }
}