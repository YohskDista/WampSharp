using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using WampSharp.Core.Utilities;
#if !VALUETASK
using ValueTask = System.Threading.Tasks.Task;
#endif

// ReSharper disable once CheckNamespace
namespace SystemEx
{
    internal class CompositeAsyncDisposable : IAsyncDisposable
    {
        private readonly IList<IAsyncDisposable> mDisposables;

        public CompositeAsyncDisposable(IList<IAsyncDisposable> disposables)
        {
            mDisposables = disposables;
        }

        public ValueTask DisposeAsync()
        {
            List<Task> tasks = new List<Task>();

            foreach (IAsyncDisposable disposable in mDisposables)
            {
                Task disposeTask = disposable.DisposeAsync().AsTask();
                tasks.Add(disposeTask);
            }

#if !NET40
            Task result = Task.WhenAll(tasks);
#else
            IObservable<Unit> whenAll = 
                from currentTask in tasks.ToObservable()
                from unit in currentTask.ToObservable()
                select unit;

            Task result = whenAll.ToTask();
#endif
            return result.AsValueTask();
        }
    }
}