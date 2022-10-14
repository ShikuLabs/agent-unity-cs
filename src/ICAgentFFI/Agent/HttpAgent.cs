namespace ICAgentFFI.Agent;

using System.Runtime.InteropServices;

public class HttpAgent
{
    public static void Create(string url)
    {
        FromRust.agent_create(url, 0);
    }

    internal static class FromRust
    {
        [DllImport("ic-agent-ffi", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void agent_create(string url, UInt32 identity);
    }
}