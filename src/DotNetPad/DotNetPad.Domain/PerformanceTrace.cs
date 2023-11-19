using System.Diagnostics;

namespace Waf.DotNetPad.Domain;

public sealed class PerformanceTrace(string name) : IDisposable
{
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();

    public PerformanceTrace(string name, DocumentFile document) : this(document.FileName + ": " + name)
    {
    }

    public void Dispose() => Trace.WriteLine(">>> " + name + ": " + stopwatch.ElapsedMilliseconds + " ms");
}
