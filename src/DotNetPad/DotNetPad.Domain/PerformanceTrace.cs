using System.Diagnostics;

namespace Waf.DotNetPad.Domain;

public sealed class PerformanceTrace : IDisposable
{
    private readonly long start;
    private readonly string name;

    public PerformanceTrace(string name)
    {
        this.name = name;
        start = Stopwatch.GetTimestamp();
    }

    public PerformanceTrace(string name, DocumentFile document) : this(document.FileName + ": " + name) {         }

    public void Dispose() => Log.Default.Trace(">>> {0}: {1} ms", name, Stopwatch.GetElapsedTime(start).TotalMilliseconds);
}
