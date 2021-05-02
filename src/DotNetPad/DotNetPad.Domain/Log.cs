using System.Diagnostics;

namespace Waf.DotNetPad.Domain
{
    public static class Log
    {
        public static TraceSource Default { get; } = new TraceSource("App");
    }
}
