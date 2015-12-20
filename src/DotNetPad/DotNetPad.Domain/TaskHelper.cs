using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Waf.DotNetPad.Domain
{
    public static class TaskHelper
    {
        public static Task Run(Action action, TaskScheduler scheduler)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach, scheduler);
        }

        public static Task WaitForProperty<T>(T observable, Func<T, bool> predicate) where T : INotifyPropertyChanged
        {
            if (predicate(observable))
            {
                return Task.FromResult((object)null);
            }
            
            var tcs = new TaskCompletionSource<object>();
            PropertyChangedEventHandler handler = (sender, e) => 
            {
                if (predicate(observable))
                {
                    tcs.SetResult(null);
                }
            };
            observable.PropertyChanged += handler;
            tcs.Task.ContinueWith(t => observable.PropertyChanged -= handler, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }
    }
}
