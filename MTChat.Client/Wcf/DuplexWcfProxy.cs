using System.ServiceModel;

namespace MTChat.Client.Wcf
{
    public class DuplexWcfProxy<TService> : DuplexClientBase<TService> 
        where TService : class 
    {
        public DuplexWcfProxy(object callbackInstance, EndpointAddress address) 
            : base(callbackInstance, new NetTcpBinding(), address)
        {
        }

        public TService WcfChannel => Channel;
    }
}
