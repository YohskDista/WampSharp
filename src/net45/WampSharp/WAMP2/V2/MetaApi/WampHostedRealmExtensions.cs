using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;
using WampSharp.V2.Testament;


namespace WampSharp.V2.MetaApi
{
    extern alias rxCore;

    public static class WampHostedRealmExtensions
    {
        /// <summary>
        /// Hosts a WAMP meta-api service describing the given realm.
        /// </summary>
        /// <param name="hostedRealm">The given realm.</param>
        /// <returns>A disposable: disposing it will unregister the hosted meta-api service.</returns>
        public static IDisposable HostMetaApiService(this IWampHostedRealm hostedRealm)
        {
            WampRealmDescriptorService service = new WampRealmDescriptorService(hostedRealm);

            rxCore::System.Reactive.Disposables.CompositeDisposable disposable = 
                HostDisposableService(hostedRealm, service, CalleeRegistrationInterceptor.Default);

            BrokerFeatures brokerFeatures = hostedRealm.Roles.Broker.Features;
            DealerFeatures dealerFeatures = hostedRealm.Roles.Dealer.Features;

            brokerFeatures.SessionMetaApi = true;
            brokerFeatures.SubscriptionMetaApi = true;
            dealerFeatures.SessionMetaApi = true;
            dealerFeatures.RegistrationMetaApi = true;

            IDisposable featureDisposable = rxCore::System.Reactive.Disposables.Disposable.Create(() =>
            {
                brokerFeatures.SessionMetaApi = null;
                brokerFeatures.SubscriptionMetaApi = null;
                dealerFeatures.SessionMetaApi = null;
                dealerFeatures.RegistrationMetaApi = null;
            });

            disposable.Add(featureDisposable);

            return disposable;
        }

        /// <summary>
        /// Hosts a WAMP testament service for the given realm.
        /// </summary>
        /// <param name="hostedRealm">The given realm.</param>
        /// <returns>A disposable: disposing it will unregister the hosted testaments service.</returns>
        public static IDisposable HostTestamentService(this IWampHostedRealm hostedRealm)
        {
            WampTestamentService service = new WampTestamentService(hostedRealm);

            RegisterOptions registerOptions = new RegisterOptions { DiscloseCaller = true };

            rxCore::System.Reactive.Disposables.CompositeDisposable disposable = HostDisposableService(hostedRealm, service, new CalleeRegistrationInterceptor(registerOptions));

            DealerFeatures dealerFeatures = hostedRealm.Roles.Dealer.Features;

            dealerFeatures.TestamentMetaApi = true;

            IDisposable featureDisposable = rxCore::System.Reactive.Disposables.Disposable.Create(() =>
            {
                dealerFeatures.TestamentMetaApi = null;
            });

            disposable.Add(featureDisposable);

            return disposable;
        }

        private static rxCore::System.Reactive.Disposables.CompositeDisposable HostDisposableService(IWampHostedRealm hostedRealm, IDisposable service, ICalleeRegistrationInterceptor registrationInterceptor)
        {
            Task<IAsyncDisposable> registrationDisposable =
                hostedRealm.Services.RegisterCallee(service, registrationInterceptor);

            IAsyncDisposable asyncDisposable = registrationDisposable.Result;

            IDisposable unregisterDisposable =
                rxCore::System.Reactive.Disposables.Disposable.Create(() => asyncDisposable.DisposeAsync().Wait());

            rxCore::System.Reactive.Disposables.CompositeDisposable result =
                new rxCore::System.Reactive.Disposables.CompositeDisposable(unregisterDisposable, service);

            return result;
        }
    }
}