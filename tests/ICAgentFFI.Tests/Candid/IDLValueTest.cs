using System.Numerics;
using ICAgentFFI.Candid;

namespace ICAgentFFI.Tests.Candid;

public class IDLValueTest
{
    [Fact]
    public void FromText_ShouldWork()
    {
        IDLValue.FromText("128: nat64");
        IDLValue.FromText("(128: nat64)");
        IDLValue.FromText("principal \"2vxsx-fae\"");
        IDLValue.FromText("(principal \"2vxsx-fae\")");
    }

    [Fact]
    public void ToString_ShouldWork()
    {
        var v01 = IDLValue.FromText("128: nat64");
        Assert.Equal("128 : nat64", v01.ToString());

        var v02 = IDLValue.FromText("(128: nat64)");
        Assert.Equal("128 : nat64", v02.ToString());

        var v03 = IDLValue.FromText("principal \"2vxsx-fae\"");
        Assert.Equal("principal \"2vxsx-fae\"", v03.ToString());

        var v04 = IDLValue.FromText("(principal \"2vxsx-fae\")");
        Assert.Equal("principal \"2vxsx-fae\"", v04.ToString());
    }

    [Fact]
    public void GetValueType_ShouldWork()
    {
        var v01 = IDLValue.FromText("128: nat64");
        Assert.Equal("nat64", v01.GetValueType());

        var v02 = IDLValue.FromText("principal \"2vxsx-fae\"");
        Assert.Equal("principal", v02.GetValueType());
    }

    [Fact]
    public void Equal_ShouldWork()
    {
        var v01 = IDLValue.FromText("true: bool");
        var v02 = IDLValue.FromText("false: bool");
        var v03 = IDLValue.FromText("-11: int32");
        var v04 = IDLValue.FromText("true: bool");

        Assert.Equal(v01, v01);
        Assert.NotEqual(v01, v02);
        Assert.NotEqual(v01, v03);
        Assert.Equal(v01, v04);

        Assert.Equal(v02, v02);
        Assert.NotEqual(v02, v03);
        Assert.NotEqual(v02, v04);

        Assert.Equal(v03, v03);
        Assert.NotEqual(v03, v04);

        Assert.Equal(v04, v04);
    }

    [Fact]
    public void AsBool_ShouldWork()
    {
        var v01 = IDLValue.FromText("true: bool");
        Assert.True(v01.AsBool());

        var v02 = IDLValue.FromText("false: bool");
        Assert.False(v02.AsBool());
    }

    [Fact]
    public void IsNull_ShouldWork()
    {
        var v01 = IDLValue.FromText("null");
        Assert.True(v01.IsNull());
    }

    [Fact]
    public void AsText_ShouldWork()
    {
        var actual = "Hello World";

        var v01 = IDLValue.FromText($"\"{actual}\": text");
        Assert.Equal(actual, v01.AsText());
    }

    [Fact]
    public void AsNumber_ShouldWork()
    {
        var actual = "123456890123456890";

        var v01 = IDLValue.FromText($"{actual}");
        Assert.Equal(actual, v01.AsNumber());
    }
    
    [Fact]
    public void AsFloat_ShouldWork()
    {
        var v01 = IDLValue.FromText("1.0: float32");
        Assert.Equal(1.0, v01.AsFloat());
    }

    [Fact]
    public void AsDouble_ShouldWork()
    {
        var v01 = IDLValue.FromText("1.03: float64");
        Assert.Equal(1.03, v01.AsDouble());
    }

    [Fact]
    public void AsOpt_ShouldWork()
    {
        var actual = IDLValue.FromText("principal \"2vxsx-fae\"");

        var v01 = IDLValue.FromText("opt principal \"2vxsx-fae\"");

        Assert.Equal(actual, v01.AsOpt());
    }

    [Fact]
    public void AsVec_ShouldWork()
    {
        var valueVec = IDLValue.FromText("vec { true; principal \"2vxsx-fae\"; 12345 }");
        var values = valueVec.AsVec();

        Assert.True(values[0].AsBool());
        Assert.Equal(Principal.Anonymous(), values[1].AsPrincipal());
        Assert.Equal("12345", values[2].AsNumber());
    }

    [Fact]
    public void AsRecord_ShouldWork()
    {
        var valueRecord = IDLValue.FromText("record { Key01 = true; 123 = principal \"2vxsx-fae\"; Key03 = 12345 }");
        var dict = valueRecord.AsRecord();

        Assert.True(dict["Key01"].AsBool());
        Assert.Equal(Principal.Anonymous(), dict["123"].AsPrincipal());
        Assert.Equal("12345", dict["Key03"].AsNumber());
    }

    [Fact]
    public void AsVariant_ShouldWork()
    {
        var valueVariant = IDLValue.FromText("variant { Key = true }");
        var (id, value) = valueVariant.AsVariant();
        
        Assert.Equal("Key", id);
        Assert.True(value.AsBool());
    }

    [Fact]
    public void AsPrincipal_ShouldWork()
    {
        var v01 = IDLValue.FromText("principal \"2vxsx-fae\"");
        Assert.Equal(Principal.Anonymous(), v01.AsPrincipal());
    }

    [Fact]
    public void AsService_ShouldWork()
    {
        var v01 = IDLValue.FromText("service \"2vxsx-fae\"");
        Assert.Equal(Principal.Anonymous(), v01.AsService());
    }
    
    [Fact]
    public void AsFunc_ShouldWork()
    {
        var valueFunc = IDLValue.FromText("func \"2vxsx-fae\".get_info");

        var (principal, funcName) = valueFunc.AsFunc();
        
        Assert.Equal(Principal.Anonymous(), principal);
        Assert.Equal("get_info", funcName);
    }

    // [Fact]
    // public void IsNone_ShouldWork()
    // {
    //     var valueNone = IDLValue.FromText("null");
    //     Assert.True(valueNone.IsNone());
    // }

    [Fact]
    public void AsInt_ShouldWork()
    {
        var num = "-12345678901234567890";
        var bi = BigInteger.Parse("-12345678901234567890");

        var value = IDLValue.FromText($"{num}: int");
        Assert.Equal(bi, value.AsInt());
    }
    
    [Fact]
    public void AsNat_ShouldWork()
    {
        var num = "12345678901234567890";
        var bi = BigInteger.Parse("12345678901234567890");

        var value = IDLValue.FromText($"{num}: nat");
        Assert.Equal(bi, value.AsNat());
    }
    
    [Fact]
    public void AsNat8_ShouldWork()
    {
        byte num = 8;

        var value = IDLValue.FromText($"{num}: nat8");
        Assert.Equal(num, value.AsNat8());
    }
    
    [Fact]
    public void AsNat16_ShouldWork()
    {
        UInt16 num = 16;

        var value = IDLValue.FromText($"{num}: nat16");
        Assert.Equal(num, value.AsNat16());
    }
    
    [Fact]
    public void AsNat32_ShouldWork()
    {
        UInt32 num = 32;

        var value = IDLValue.FromText($"{num}: nat32");
        Assert.Equal(num, value.AsNat32());
    }
    
    [Fact]
    public void AsNat64_ShouldWork()
    {
        UInt64 num = 64;

        var value = IDLValue.FromText($"{num}: nat64");
        Assert.Equal(num, value.AsNat64());
    }
    
    [Fact]
    public void AsInt8_ShouldWork()
    {
        sbyte num = -8;

        var value = IDLValue.FromText($"{num}: int8");
        Assert.Equal(num, value.AsInt8());
    }
    
    [Fact]
    public void AsInt16_ShouldWork()
    {
        Int16 num = -16;

        var value = IDLValue.FromText($"{num}: int16");
        Assert.Equal(num, value.AsInt16());
    }
    
    [Fact]
    public void AsInt32_ShouldWork()
    {
        Int32 num = -32;

        var value = IDLValue.FromText($"{num}: int32");
        Assert.Equal(num, value.AsInt32());
    }
    
    [Fact]
    public void AsInt64_ShouldWork()
    {
        Int64 num = -64;

        var value = IDLValue.FromText($"{num}: int64");
        Assert.Equal(num, value.AsInt64());
    }

    [Fact]
    public void IsReserved_ShouldWork()
    {
        var valueReserved = IDLValue.FromText("null : reserved");
        Assert.True(valueReserved.IsReserved());
    }
}