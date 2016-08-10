using System;
using MTChat.Client.Wcf;
using MTChat.Common;
using MTChat.Common.Messages;

namespace MTChat.Client.Services
{
    public class LocalChatService : ILocalChatService
    {
        private ChatServiceProxy _serviceProxy;
        public event EventHandler<ProxyCallbackEventArgs> ChatCallbackEvent;

        public void Say(TextMessage msg)
        {
            _serviceProxy.Say(msg);
        }

        public void Whisper(PersonalTextMessage msg)
        {
            _serviceProxy.Whisper(msg);
        }

        public Person[] Join(Person person, string serverIp, string port)
        {
            var chatCallback = new ChatServiceCallback();
            chatCallback.ChatCallbackEvent += ChatCallbackEvent;
            var uri = new Uri($"net.tcp://{serverIp}:{port}");
            _serviceProxy = new ChatServiceProxy(chatCallback, uri);
            var result = _serviceProxy.Join(person);
            return result.Result;
        }

        public void Leave()
        {
            _serviceProxy.Leave();
        }
    }
}
