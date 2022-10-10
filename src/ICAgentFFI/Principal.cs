using System.Runtime.InteropServices;

using StateCode = System.Int32;

namespace IC
{
    public struct Principal
    {
        public static void Test()
        {
            byte[] out_arr = new byte[64];
            UInt32 out_arr_len = 0;

            {
                var sc = FFIMethods.principal_management_canister(out_arr, out out_arr_len, (UInt32)out_arr.Length);
                Console.WriteLine($"{sc}: {string.Join(",", out_arr)}, {out_arr_len}");
            }

            {
                var sc = FFIMethods.principal_anonymous(out_arr, out out_arr_len, (UInt32)out_arr.Length);
                Console.WriteLine($"{sc}: {string.Join(",", out_arr)}, {out_arr_len}");
            }
        }
    }

    internal static class FFIMethods
    {
        internal const StateCode SC_OK = 0;
        internal const StateCode SC_DATA_OVERFLOW = -1;
        internal const StateCode SC_INTERNAL_ERR = -2;
        internal const StateCode SC_ERR_INFO_OVERFLOW = -3;

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_management_canister(byte[] out_arr, out UInt32 out_arr_len, in UInt32 arr_len);

        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern StateCode principal_anonymous(byte[] out_arr, out UInt32 out_arr_len, in UInt32 arr_len);
    }
}