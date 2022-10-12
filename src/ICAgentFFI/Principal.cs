using System.Numerics;
using System.Runtime.InteropServices;
using StateCode = System.Int32;

namespace ICAgentFFI
{
    public class Principal : IEquatable<Principal>
    {
        private const byte NullTerminated = 0;

        private static UInt32 OutArrSize = 32;
        private static UInt32 OutTextSize = 128;
        private static UInt32 OutErrInfoSize = 256;

        public byte[] Bytes { get; }

        Principal(byte[] arr, UInt32 arrLen)
        {
            var bytes = new byte[arrLen];
            Array.Copy(arr, bytes, arrLen);
            Bytes = bytes;
        }

        public static Principal ManagementCanister()
        {
            byte[] outArr = new byte[OutArrSize];

            var sc = FromRust.principal_management_canister(outArr, out UInt32 outArrLen, (UInt32)outArr.Length);

            switch (sc)
            {
                case FromRust.ScOk:
                    return new Principal(outArr, outArrLen);
                case FromRust.ScDataOverflow:
                    throw new DataOverflowException("Data Overflow: Unable to take off the data of principal bytes.");
                default:
                    throw new UnknownException("Unknown: The StateCode returned is unexpected.");
            }
        }

        public static Principal SelfAuthenticating(byte[] publicKey)
        {
            byte[] outArr = new byte[OutArrSize];

            var sc = FromRust.principal_self_authenticating(
                outArr,
                out UInt32 outArrLen,
                (UInt32)outArr.Length,
                publicKey,
                (UInt32)publicKey.Length
            );

            switch (sc)
            {
                case FromRust.ScOk:
                    return new Principal(outArr, outArrLen);
                case FromRust.ScDataOverflow:
                    throw new DataOverflowException("Data Overflow: Unable to take off the data of principal bytes.");
                default:
                    throw new UnknownException("Unknown: The StateCode returned is unexpected.");
            }
        }

        public static Principal Anonymous()
        {
            byte[] outArr = new byte[OutArrSize];

            var sc = FromRust.principal_anonymous(
                outArr,
                out UInt32 outArrLen,
                (UInt32)outArr.Length
            );

            switch (sc)
            {
                case FromRust.ScOk:
                    return new Principal(outArr, outArrLen);
                case FromRust.ScDataOverflow:
                    throw new DataOverflowException("Data Overflow: Unable to take off the data of principal bytes.");
                default:
                    throw new UnknownException("Unknown: The StateCode returned is unexpected.");
            }
        }

        public static Principal FromBytes(byte[] bytes)
        {
            byte[] outArr = new byte[OutArrSize];
            byte[] outErrInfo = new byte[OutErrInfoSize];

            var sc = FromRust.principal_from_bytes(
                bytes,
                (UInt32)bytes.Length,
                outArr,
                out UInt32 outArrLen,
                (UInt32)outArr.Length,
                outErrInfo,
                (UInt32)outErrInfo.Length
            );

            switch (sc)
            {
                case FromRust.ScOk:
                    return new Principal(outArr, outArrLen);
                case FromRust.ScDataOverflow:
                    throw new DataOverflowException("Data Overflow: Unable to take off the data of principal bytes.");
                case FromRust.ScInternalErr:
                    var len = Array.IndexOf(outErrInfo, NullTerminated);
                    var errInfo = System.Text.Encoding.ASCII.GetString(outErrInfo, 0, len);
                    throw new InternalErrorException($"Internal: {errInfo}");
                case FromRust.ScErrInfoOverflow:
                    throw new ErrInfoOverflowException("ErrInfo Overflow: Unable to take off the data of error.");
                default:
                    throw new UnknownException("Unknown: The StateCode returned is unexpected.");
            }
        }

        public static Principal FromText(string text)
        {
            byte[] outArr = new byte[OutArrSize];
            byte[] outErrInfo = new byte[OutErrInfoSize];

            var sc = FromRust.principal_from_text(
                text,
                outArr,
                out UInt32 outArrLen,
                (UInt32)outArr.Length,
                outErrInfo,
                (UInt32)outErrInfo.Length
            );

            switch (sc)
            {
                case FromRust.ScOk:
                    return new Principal(outArr, outArrLen);
                case FromRust.ScDataOverflow:
                    throw new DataOverflowException("Data Overflow: Unable to take off the data of principal bytes.");
                case FromRust.ScInternalErr:
                    var len = Array.IndexOf(outErrInfo, NullTerminated);
                    var errInfo = System.Text.Encoding.ASCII.GetString(outErrInfo, 0, len);
                    throw new InternalErrorException($"Internal: {errInfo}");
                case FromRust.ScErrInfoOverflow:
                    throw new ErrInfoOverflowException("ErrInfo Overflow: Unable to take off the data of error.");
                default:
                    throw new UnknownException("Unknown: The StateCode returned is unexpected.");
            }
        }

        public override string ToString()
        {
            byte[] outText = new byte[OutTextSize];
            byte[] outErrInfo = new byte[OutErrInfoSize];

            var sc = FromRust.principal_to_text(
                Bytes,
                (UInt32)Bytes.Length,
                outText,
                (UInt32)outText.Length,
                outErrInfo,
                (UInt32)outErrInfo.Length
            );

            switch (sc)
            {
                case FromRust.ScOk:
                {
                    var len = Array.IndexOf(outText, NullTerminated);
                    var principalText = System.Text.Encoding.ASCII.GetString(outText, 0, len);
                    return principalText;
                }
                case FromRust.ScDataOverflow:
                    throw new DataOverflowException("Data Overflow: Unable to take off the data of principal text.");
                case FromRust.ScInternalErr:
                {
                    var len = Array.IndexOf(outErrInfo, NullTerminated);
                    var errInfo = System.Text.Encoding.ASCII.GetString(outErrInfo, 0, len);
                    throw new InternalErrorException($"Internal: {errInfo}");
                }
                case FromRust.ScErrInfoOverflow:
                    throw new ErrInfoOverflowException("ErrInfo Overflow: Unable to take off the data of error.");
                default:
                    throw new UnknownException("Unknown: The StateCode returned is unexpected.");
            }
        }

        public bool Equals(Principal? principal)
        {
            if (principal == null) return false;

            if (GetType() != principal.GetType()) return false;

            if (ReferenceEquals(this, principal)) return true;

            return Enumerable.SequenceEqual(Bytes, principal.Bytes);
        }

        public override bool Equals(object? obj) => Equals(obj as Principal);

        public override int GetHashCode()
        {
            return new BigInteger(Bytes).GetHashCode();
        }
    }

    public class DataOverflowException : Exception
    {
        public DataOverflowException()
        {
        }

        public DataOverflowException(string message) : base(message)
        {
        }

        public DataOverflowException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class InternalErrorException : Exception
    {
        public InternalErrorException()
        {
        }

        public InternalErrorException(string message) : base(message)
        {
        }

        public InternalErrorException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class ErrInfoOverflowException : Exception
    {
        public ErrInfoOverflowException()
        {
        }

        public ErrInfoOverflowException(string message) : base(message)
        {
        }

        public ErrInfoOverflowException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class UnknownException : Exception
    {
        public UnknownException()
        {
        }

        public UnknownException(string message) : base(message)
        {
        }

        public UnknownException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    internal static class FromRust
    {
        internal const StateCode ScOk = 0;
        internal const StateCode ScDataOverflow = -1;
        internal const StateCode ScInternalErr = -2;
        internal const StateCode ScErrInfoOverflow = -3;

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_management_canister(
            byte[] outArr,
            out UInt32 outArrLen,
            UInt32 arrSize
        );

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_self_authenticating(
            byte[] outArr,
            out UInt32 outArrLen,
            UInt32 arrSize,
            byte[] publicKey,
            UInt32 publicKeySize
        );

        [DllImport("ic-agent-ffi",
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_anonymous(
            byte[] outArr,
            out UInt32 outArrLen,
            UInt32 arrLen
        );

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_from_bytes(
            byte[] bytes,
            UInt32 bytesSize,
            byte[] outArr,
            out UInt32 outArrLen,
            UInt32 arrSize,
            byte[] outErrInfo,
            UInt32 arrInfoSize
        );

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_from_text(
            string text,
            byte[] outArr,
            out UInt32 outArrLen,
            UInt32 arrSize,
            byte[] outErrInfo,
            UInt32 errInfoSize
        );

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_to_text(
            byte[] bytes,
            UInt32 bytesSize,
            byte[] outText,
            UInt32 textSize,
            byte[] outErrInfo,
            UInt32 errInfoSize
        );
    }
}