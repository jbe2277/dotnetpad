using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Waf.DotNetPad.Domain
{
    public static class TaskUtility
    {
        public static Task WaitForProperty<T>(T observable, Func<T, bool> predicate) where T : INotifyPropertyChanged
        {
            if (predicate(observable)) return Task.CompletedTask;
            
            var tcs = new TaskCompletionSource<object?>();
            void Handler(object sender, PropertyChangedEventArgs e)
            {
                if (predicate(observable))
                {
                    tcs.SetResult(null);
                }
            }
            observable.PropertyChanged += Handler;
            tcs.Task.ContinueWith(t => observable.PropertyChanged -= Handler, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }
    }
}
