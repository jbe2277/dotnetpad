using System.Diagnostics;

namespace Waf.DotNetPad.Domain;

public sealed class PerformanceTrace(string name) : IDisposable
{
    private readonly long start;

    public PerformanceTrace(string name, DocumentFile document) : this(document.FileName + ": " + name)
    {
        start = Stopwatch.GetTimestamp();
    }

    public void Dispose() => Log.Default.Trace(">>> {0}: {1} ms", name, Stopwatch.GetElapsedTime(start));
}
