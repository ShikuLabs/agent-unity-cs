namespace ICAgentFFI;

public enum StateCode
{
    Ok = 0,
    Err = -1,
}

public static class Config
{
    public const byte NullTerminated = 0;
    
    public const UInt32 OutArrSize = 32;
    public const UInt32 OutTextSize = 128;
    public const UInt32 OutErrInfoSize = 256;
}