using System;
using System.ServiceModel;
using MTChat.Common;

namespace MTChat.Client.Wcf
{
    public abstract class WcfAdapter<TContract> where TContract : class 
    {
        private readonly DuplexWcfProxy<TContract> _proxy;

        protected WcfAdapter(object callbackInstance,Uri  address)
        {
            var endpointAddress = new EndpointAddress(address);
            _proxy = new DuplexWcfProxy<TContract>(callbackInstance, endpointAddress);
        }

        protected TResult ExecuteCommand<TResult>(Func<TContract, TResult> command)
            where TResult : OperationResult
        {
            return WcfRequestHandler.RequestHandle(_proxy, command);
        }
    }
}
