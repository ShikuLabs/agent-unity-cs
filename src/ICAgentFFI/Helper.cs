namespace ICAgentFFI;

public enum StateCode
{
    Ok = 0,
    DataOverflow = -1,
    InternalErr = -2,
    ErrInfoOverflow = -3,
}

public static class Config
{
    public const byte NullTerminated = 0;
    
    public static UInt32 OutArrSize = 32;
    public static UInt32 OutTextSize = 128;
    public static UInt32 OutErrInfoSize = 256;

    public static void Reset()
    {
        OutArrSize = 32;
        OutTextSize = 128;
        OutErrInfoSize = 256;
    }
}