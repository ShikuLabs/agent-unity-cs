namespace ICAgentFFI;

using System.Runtime.InteropServices;

public class Identity
{
    private IntPtr[] fptr;

    public IdentityType Type { get; }

    private Identity(IntPtr[] fptr, IdentityType type)
    {
        this.fptr = fptr;
        this.Type = type;
    }
    
    ~Identity()
    {
        FromRust.identity_free(fptr);
    }
    
    /// <summary>
    /// Create an [`Identity`] with `Anonymous` type.
    /// </summary>
    public static Identity Anonymous()
    {
        var outFptr = new IntPtr[2];

        FromRust.identity_anonymous(outFptr);

        return new Identity(outFptr, IdentityType.Anonymous);
    }

    /// <summary>
    /// Create an [`Identity`] with `Basic` type.
    /// </summary>
    public static Identity BasicRandom()
    {
        var outFptr = new IntPtr[2];
        var outErrInfo = new byte[Config.OutErrInfoSize];

        var sc = FromRust.identity_basic_random(outFptr, outErrInfo, (UInt32)outErrInfo.Length);
        
        switch (sc)
        {
            case StateCode.Ok:
                return new Identity(outFptr, IdentityType.Basic);
            case StateCode.DataOverflow:
                throw new DataOverflowException("Data Overflow: Unable to take off the data of principal bytes.");
            case StateCode.InternalErr:
                var len = Array.IndexOf(outErrInfo, Config.NullTerminated);
                var errInfo = System.Text.Encoding.ASCII.GetString(outErrInfo, 0, len);
                throw new InternalErrorException($"Internal: {errInfo}");
            case StateCode.ErrInfoOverflow:
                throw new ErrInfoOverflowException("ErrInfo Overflow: Unable to take off the data of error.");
            default:
                throw new UnknownException("Unknown: The StateCode returned is unexpected.");
        }
    }

    public Principal Sender()
    {
        byte[] outArr = new byte[Config.OutArrSize];
        byte[] outErrInfo = new byte[Config.OutErrInfoSize];

        var sc = FromRust.identity_sender(
            fptr,
            outArr,
            out UInt32 outArrLen,
            (UInt32)outArr.Length,
            outErrInfo,
            (UInt32)outErrInfo.Length
        );
        
        switch (sc)
        {
            case StateCode.Ok:
                return new Principal(outArr, outArrLen);
            case StateCode.DataOverflow:
                throw new DataOverflowException("Data Overflow: Unable to take off the data of principal bytes.");
            case StateCode.InternalErr:
                var len = Array.IndexOf(outErrInfo, Config.NullTerminated);
                var errInfo = System.Text.Encoding.ASCII.GetString(outErrInfo, 0, len);
                throw new InternalErrorException($"Internal: {errInfo}");
            case StateCode.ErrInfoOverflow:
                throw new ErrInfoOverflowException("ErrInfo Overflow: Unable to take off the data of error.");
            default:
                throw new UnknownException("Unknown: The StateCode returned is unexpected.");
        }
    }

    internal static class FromRust
    {
        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode identity_anonymous(IntPtr[] outFPtr);
        
        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode identity_basic_random(
            IntPtr[] outFPtr,
            byte[] outErrInfo,
            UInt32 errInfoSize
        );
        
        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode identity_basic_from_pem_file(
            string path,
            IntPtr[] outFPtr,
            byte[] outErrInfo,
            UInt32 errInfoSize
        );
        
        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode identity_basic_from_pem(
            string pem,
            IntPtr[] outFPtr,
            byte[] outErrInfo,
            UInt32 errInfoSize
        );
        
        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode identity_secp256k1_random(
            IntPtr[] outFPtr,
            byte[] outErrInfo,
            UInt32 errInfoSize
        );
        
        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode identity_secp256k1_from_pem_file(
            string path,
            IntPtr[] outFPtr,
            byte[] outErrInfo,
            UInt32 errInfoSize
        );
        
        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode identity_secp256k1_from_pem(
            string pem,
            IntPtr[] outFPtr,
            byte[] outErrInfo,
            UInt32 errInfoSize
        );
        
        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode identity_sender(
            IntPtr[] fptr,
            byte[] outArr,
            out UInt32 outArrLen,
            UInt32 arrSize,
            byte[] outErrInfo,
            UInt32 errInfoSize
        );
        
        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode identity_sign(
            IntPtr[] fptr,
            byte[] bytes,
            UInt32 bytesLen,
            byte[] outPublicKey,
            out UInt32 outPublicKeyLen,
            UInt32 publicKeySize,
            byte[] outSignature,
            out UInt32 outSignatureLen,
            UInt32 signatureSize,
            byte[] outErrInfo,
            UInt32 errInfoSize
        );
        
        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode identity_free(IntPtr[] fptr);
    }
}

public enum IdentityType
{
    Anonymous = 0,
    Basic = 1,
    Secp256K1 = 2,
}