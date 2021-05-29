using System;
using System.Diagnostics;

namespace Waf.DotNetPad.Domain
{
    public sealed class PerformanceTrace : IDisposable
    {
        private readonly Stopwatch stopwatch;
        private readonly string name;

        public PerformanceTrace(string name)
        {
            stopwatch = Stopwatch.StartNew();
            this.name = name;
        }

        public PerformanceTrace(string name, DocumentFile document) : this(document.FileName + ": " + name)
        {
        }

        public void Dispose() => Trace.WriteLine(">>> " + name + ": " + stopwatch.ElapsedMilliseconds + " ms");
    }
}
