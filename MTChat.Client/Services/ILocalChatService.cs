using System;
using MTChat.Common;
using MTChat.Common.Messages;

namespace MTChat.Client.Services
{
    public interface ILocalChatService
    {
        event EventHandler<ProxyCallbackEventArgs> ChatCallbackEvent;
        Person[] Join(Person person, string serverIp, string port);
        void Leave();
        void Say(TextMessage msg);
        void Whisper(PersonalTextMessage msg);
    }
}
