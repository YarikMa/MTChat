using System;
using System.ServiceModel;
using MTChat.Common;

namespace MTChat.Client.Wcf
{
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class ChatServiceCallback : IChatServiceCallback
    {
        public event EventHandler<ProxyCallbackEventArgs> ChatCallbackEvent;

        public void Receive(Person sender, string message)
        {
            ProcessCallback(sender, CallbackType.Receive, message);
        }

        public void ReceiveWhisper(Person sender, string message)
        {
            ProcessCallback(sender, CallbackType.ReceiveWhisper, message);
        }

        public void UserEnter(Person person)
        {
            ProcessCallback(person, CallbackType.UserEnter);
        }

        public void UserLeave(Person person)
        {
            ProcessCallback(person, CallbackType.UserLeave);
        }

        public void DisconnectByTimeout()
        {
            ProcessCallback(null, CallbackType.DisconnectByTimeout);
        }

        private void ProcessCallback(Person person, CallbackType type, string message = "")
        {
            var args = new ProxyCallbackEventArgs
            {
                Person = person,
                CallbackType = type,
                Message = message
            };
            OnChatCallbackEvent(args);
        }

        private void OnChatCallbackEvent(ProxyCallbackEventArgs args)
        {
            ChatCallbackEvent?.Invoke(this, args);
        }
    }
}
