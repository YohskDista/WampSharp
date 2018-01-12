using WampSharp.V2.Rpc;

namespace WampSharp.V2.Management
{
    public interface IManagementService
    {
        [WampProcedure("wamp.session.kill")]
        void KillSession(long sessionId, string reason, string message);

        [WampProcedure("wamp.registration.remove_callee")]
        void RemoveCallee(long registrationId, long calleeId, string reason);

        [WampProcedure("wamp.subscription.remove_subscriber")]
        void RemoveSubscriber(long subscriptionId, long subscriberId, string reason);
    }

    public class ManagementService
    {
        
    }
}