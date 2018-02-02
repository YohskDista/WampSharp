using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using WampSharp.Core.Listener;

namespace WampSharp.V2
{
    extern alias rxCore;

    internal class CompositeListener<TMessage> : IWampConnectionListener<TMessage>
    {
        private readonly IEnumerable<IWampConnectionListener<TMessage>> mListeners;

        public CompositeListener(IEnumerable<IWampConnectionListener<TMessage>> listeners)
        {
            mListeners = listeners;
        }

        public IDisposable Subscribe(IObserver<IWampConnection<TMessage>> observer)
        {
            rxCore::System.Reactive.Disposables.CompositeDisposable disposable = new rxCore::System.Reactive.Disposables.CompositeDisposable();

            foreach (IWampConnectionListener<TMessage> listener in mListeners)
            {
                IDisposable subscription = listener.Subscribe(observer);
                disposable.Add(subscription);
            }

            return disposable;
        }
    }
}