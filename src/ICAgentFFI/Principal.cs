using System.Runtime.InteropServices;

using StateCode = System.Int32;

namespace ICAgentFFI
{
    public class Principal
    {
        private static byte[] OUT_ARR = new byte[32];
        static uint OUT_ARR_LEN = 0;
        static byte[] OUT_ERR_INFO = new byte[256];

        static void ClearOutVars()
        {
            Array.Clear(OUT_ARR);
            OUT_ARR_LEN = 0;

            Array.Clear(OUT_ERR_INFO);
        }

        public byte[] Bytes { get; }

        Principal()
        {
            var bytes = new byte[OUT_ARR_LEN];
            Array.Copy(OUT_ARR, bytes, OUT_ARR_LEN);
            this.Bytes = bytes;
        }

        public static Principal ManagementCanister()
        {
            ClearOutVars();

            var sc = FromRust.principal_management_canister(OUT_ARR, out OUT_ARR_LEN, (UInt32)OUT_ARR.Length);

            // If state code is out of range, make it fail.
            if (sc != FromRust.SC_OK && sc != FromRust.SC_DATA_OVERFLOW) sc = Int32.MaxValue;

            return Principal.PostProcess(sc);
        }

        public static Principal SelfAuthenticating(byte[] publicKey)
        {
            ClearOutVars();

            var sc = FromRust.principal_self_authenticating(OUT_ARR, out OUT_ARR_LEN, (UInt32)OUT_ARR.Length, publicKey,
                (UInt32)publicKey.Length);

            // If state code is out of range, make it fail.
            if (sc != FromRust.SC_OK && sc != FromRust.SC_DATA_OVERFLOW) sc = Int32.MaxValue;

            return Principal.PostProcess(sc);
        }

        public static Principal Anonymous()
        {
            ClearOutVars();

            var sc = FromRust.principal_anonymous(OUT_ARR, out OUT_ARR_LEN, (UInt32)OUT_ARR.Length);

            // If state code is out of range, make it fail.
            if (sc != FromRust.SC_OK && sc != FromRust.SC_DATA_OVERFLOW) sc = Int32.MaxValue;

            return Principal.PostProcess(sc);
        }

        public static Principal FromBytes(byte[] bytes)
        {
            ClearOutVars();

            var sc = FromRust.principal_from_bytes(
                bytes,
                (UInt32)bytes.Length,
                OUT_ARR,
                out OUT_ARR_LEN,
                (UInt32)OUT_ARR.Length,
                OUT_ERR_INFO,
                (UInt32)OUT_ERR_INFO.Length
            );

            // If state code is out of range, make it fail.
            if (
                sc != FromRust.SC_OK &&
                sc != FromRust.SC_DATA_OVERFLOW &&
                sc != FromRust.SC_INTERNAL_ERR &&
                sc != FromRust.SC_ERR_INFO_OVERFLOW
            ) sc = Int32.MaxValue;

            return Principal.PostProcess(sc);
        }

        public static Principal FromText(string text)
        {
            ClearOutVars();

            var sc = FromRust.principal_from_text(
                text,
                OUT_ARR,
                out OUT_ARR_LEN,
                (UInt32)OUT_ARR.Length,
                OUT_ERR_INFO,
                (UInt32)OUT_ERR_INFO.Length
            );

            // If state code is out of range, make it fail.
            if (
                sc != FromRust.SC_OK &&
                sc != FromRust.SC_DATA_OVERFLOW &&
                sc != FromRust.SC_INTERNAL_ERR &&
                sc != FromRust.SC_ERR_INFO_OVERFLOW
            ) sc = Int32.MaxValue;

            return Principal.PostProcess(sc);
        }

        /// Helper Function: Construct Result by StateCode and OUT Vars.
        static Principal PostProcess(StateCode sc)
        {
            switch (sc)
            {
                case FromRust.SC_OK:
                    return new Principal();
                case FromRust.SC_DATA_OVERFLOW:
                    throw new Exception($"Data Overflow: `OUT_ARR_LEN({OUT_ARR_LEN})` > `ARR_SIZE({OUT_ARR.Length})`.");
                case FromRust.SC_INTERNAL_ERR:
                    var len = Array.IndexOf(OUT_ERR_INFO, '\0');
                    var errInfo = System.Text.Encoding.ASCII.GetString(OUT_ERR_INFO, 0, len);
                    throw new Exception($"Internal: {errInfo}");
                case FromRust.SC_ERR_INFO_OVERFLOW:
                    throw new Exception(
                        $"ErrInfo Overflow: The length of ErrInfo is bigger than `ERR_INFO_SIZE({OUT_ERR_INFO.Length})`.");
                default:
                    throw new Exception("Unknown: The StateCode returned is unexpected.");
            }

            ;
        }

        public override string ToString()
        {
            ClearOutVars();

            var OUT_TEXT = new byte[64];

            var sc = FromRust.principal_to_text(
                this.Bytes,
                (UInt32)this.Bytes.Length,
                OUT_TEXT,
                (UInt32)OUT_TEXT.Length,
                OUT_ERR_INFO,
                (UInt32)OUT_ERR_INFO.Length
            );

            switch (sc)
            {
                case FromRust.SC_OK:
                {
                    var len = Array.IndexOf(OUT_TEXT, (byte)0);
                    var principalText = System.Text.Encoding.ASCII.GetString(OUT_TEXT, 0, len);
                    return principalText;
                }
                case FromRust.SC_DATA_OVERFLOW:
                {
                    throw new Exception(
                        $"Data Overflow: The length of textual Principal is bigger than `OUT_TEXT({OUT_ERR_INFO.Length})`.`.");
                }
                case FromRust.SC_INTERNAL_ERR:
                {
                    var len = Array.IndexOf(OUT_ERR_INFO, '\0');
                    var errInfo = System.Text.Encoding.ASCII.GetString(OUT_ERR_INFO, 0, len);
                    throw new Exception($"Internal: {errInfo}");
                }
                case FromRust.SC_ERR_INFO_OVERFLOW:
                {
                    throw new Exception(
                        $"ErrInfo Overflow: The length of ErrInfo is bigger than `ERR_INFO_SIZE({OUT_ERR_INFO.Length})`.");
                }
                default:
                    throw new Exception("Unknown: The StateCode returned is unexpected.");
            }
        }
    }

    internal static class FromRust
    {
        internal const StateCode SC_OK = 0;
        internal const StateCode SC_DATA_OVERFLOW = -1;
        internal const StateCode SC_INTERNAL_ERR = -2;
        internal const StateCode SC_ERR_INFO_OVERFLOW = -3;

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_management_canister(byte[] out_arr, out UInt32 out_arr_len,
            UInt32 arr_len);

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_self_authenticating(
            byte[] out_arr,
            out UInt32 out_arr_len,
            UInt32 arr_size,
            byte[] public_key,
            UInt32 public_key_size
        );

        [DllImport("ic-agent-ffi",
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_anonymous(byte[] out_arr, out UInt32 out_arr_len, UInt32 arr_len);

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_from_bytes(
            byte[] bytes,
            UInt32 bytes_size,
            byte[] out_arr,
            out UInt32 out_arr_len,
            UInt32 arr_size,
            byte[] out_err_info,
            UInt32 err_info_size
        );

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_from_text(
            string text,
            byte[] out_arr,
            out UInt32 out_arr_len,
            UInt32 arr_size,
            byte[] out_err_info,
            UInt32 err_info_size
        );

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_to_text(
            byte[] bytes,
            UInt32 bytes_size,
            byte[] out_text,
            UInt32 text_size,
            byte[] out_err_info,
            UInt32 err_info_size
        );
    }
}